using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Class building and solving admittance matrices for <see cref="ISchematic"/>s
	/// </summary>
	public class AdmittanceMatrix
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		private AdmittanceMatrix(ISchematic schematic)
		{
			_Schematic = schematic ?? throw new ArgumentNullException(nameof(schematic));
		}

		#endregion

		#region Private properties

		/// <summary>
		/// True if the circuit is DC only (there are no <see cref="IACVoltageSource"/>s)
		/// </summary>
		private bool _IsPureDC => !_VoltageSources.Exists((source) => source is IACVoltageSource);

		/// <summary>
		/// True if the circuit is AC only (there are no <see cref="IOpAmp"/>s, no <see cref="ICurrentSource"/>s and
		/// every <see cref="IVoltageSource"/> is an <see cref="IACVoltageSource"/> with <see cref="IVoltageSource.ProducedDCVoltage"/>
		/// equal to 0)
		/// </summary>
		private bool _IsPureAC => _OpAmps.Count == 0 && _CurrentSources.Count == 0 &&
			_VoltageSources.All((source) => source is IACVoltageSource && source.ProducedDCVoltage == 0);

		/// <summary>
		/// Schematic on which the matrix is based
		/// </summary>
		private ISchematic _Schematic { get; }

		/// <summary>
		/// List with all nodes generated for this <see cref="AdmittanceMatrix"/>, order is important - position in the list indicates
		/// the index of the node which directly affects the admittance matrix
		/// </summary>
		private List<INode> _Nodes { get; set; }

		/// <summary>
		/// List with all voltage sources in the <see cref="_Schematic"/>, order is important - position in the list indicates the
		/// index of the source which directly affects the admittance matrix. Does not include op amp outputs
		/// </summary>
		private List<IVoltageSource> _VoltageSources { get; set; }

		/// <summary>
		/// Dictionary with voltage sources and indexes of their nodes (in order: negative, positive). If a node is grounded
		/// then it is given by -1
		/// </summary>
		private Dictionary<IVoltageSource, Tuple<int, int>> _VoltageSourcesNodes { get; set; } =
			new Dictionary<IVoltageSource, Tuple<int, int>>();

		/// <summary>
		/// List with all voltage sources in the <see cref="_Schematic"/>, order is important - position in the list indicates the
		/// index of the source which directly affects the admittance matrix. Does not include op amp outputs
		/// </summary>
		private List<ICurrentSource> _CurrentSources { get; set; }

		/// <summary>
		/// Dictionary with current sources and indexes of their nodes (in order: negative, positive). If a node is grounded
		/// then it is given by -1
		/// </summary>
		private Dictionary<ICurrentSource, Tuple<int, int>> _CurrentSourcesNodes { get; set; } =
			new Dictionary<ICurrentSource, Tuple<int, int>>();

		/// <summary>
		/// List with all op-amps in the <see cref="_Schematic"/>, order is important - position in the list indicates the
		/// index of the op-amp which directly affects the admittance matrix
		/// </summary>
		private List<IOpAmp> _OpAmps { get; set; }

		/// <summary>
		/// Dictionary with op-amps and indexes of their nodes (in order: non-inverting, inverting and output). If a node is grounded
		/// then it is given by -1
		/// </summary>
		private Dictionary<IOpAmp, Tuple<int, int, int>> _OpAmpNodes { get; set; } = new Dictionary<IOpAmp, Tuple<int, int, int>>();

		/// <summary>
		/// Size of G part of the admittance matrix (dependent on nodes)
		/// </summary>
		private int _BigDimension => _Nodes.Count;

		/// <summary>
		/// Size of D part of admittance matrix (depends on the number of independent voltage sources, with op-amp outputs included)
		/// </summary>
		private int _SmallDimension => _VoltageSources.Count + _OpAmps.Count;
		
		/// <summary>
		/// Size of the whole admittance matrix
		/// </summary>
		private int _Size => _BigDimension + _SmallDimension;

		/// <summary>
		/// Part of admittance matrix located in the top left corner, built based on nodes and admittances connected to them
		/// </summary>
		private Complex[,] _G { get; set; }

		/// <summary>
		/// Part of admittance matrix located in the top right corner - based on independent voltage sources (including op-amp outputs)
		/// </summary>
		private int[,] _B { get; set; }

		/// <summary>
		/// Part of admittance matrix located in the bottom left corner - based on independent voltage sources (excluding op-amp outputs)
		/// and inputs of op-amps
		/// </summary>
		private Complex[,] _C { get; set; }

		/// <summary>
		/// Part of admittance matrix located in the bottom right corner - based on dependent sources
		/// </summary>
		private Complex[,] _D { get; set; }

		/// <summary>
		/// Top part of the vector of free terms, based on current sources
		/// </summary>
		private Complex[] _I { get; set; }

		/// <summary>
		/// Bottom part of the vector of free terms, based on voltage sources (including op-amp outputs)
		/// </summary>
		private Complex[] _E { get; set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Helper of <see cref="Build"/>, constructs nodes, sets all potentials to 0, finds and removes reference nodes
		/// </summary>
		private void ConstructNodes()
		{
			// Generate nodes using helper class
			_Nodes = NodeGenerator.Generate(_Schematic);

			// Assign the potentials from the nodes to the associated terminals
			_Nodes.ForEach((node) => node.ConnectedTerminals.ForEach((terminal) => terminal.Potential = node.Potential));

			// Find, assign and remove the reference (ground) nodes
			ProcessReferenceNodes(_Nodes);
		}

		/// <summary>
		/// Extracts all components that require special care (<see cref="IVoltageSource"/>s, <see cref="ICurrentSource"/>s,
		/// <see cref="IOpAmp"/>s) to their respective containers
		/// </summary>
		private void ExtractSpecialComponents()
		{
			// Get the voltage sources
			_VoltageSources = new List<IVoltageSource>(_Schematic.Components.Where((component) => component is IVoltageSource).
				Cast<IVoltageSource>());

			// Get the current sources
			_CurrentSources = new List<ICurrentSource>(_Schematic.Components.Where((component) => component is ICurrentSource).
				Cast<ICurrentSource>());

			// Get the op-amps
			_OpAmps = new List<IOpAmp>(_Schematic.Components.Where((component) => component is IOpAmp).Cast<IOpAmp>());
		}

		/// <summary>
		/// Creates and initializes sub matrices (<see cref="_G"/>, <see cref="_B"/>, <see cref="_C"/>, <see cref="_D"/>
		/// <see cref="_E"/>, <see cref="_I"/>) with default values (zeros)
		/// </summary>
		private void InitializeSubMatrices()
		{
			_G = ArrayHelpers.CreateAndInitialize(Complex.Zero, _BigDimension, _BigDimension);
			_B = ArrayHelpers.CreateAndInitialize(0, _BigDimension, _SmallDimension);
			_C = ArrayHelpers.CreateAndInitialize(Complex.Zero, _SmallDimension, _BigDimension);
			_D = ArrayHelpers.CreateAndInitialize(Complex.Zero, _SmallDimension, _SmallDimension);
			_I = ArrayHelpers.CreateAndInitialize(Complex.Zero, _Size);
			_E = ArrayHelpers.CreateAndInitialize(Complex.Zero, _Size);
		}

		/// <summary>
		/// Constructs the intial version of the admittance matrix (which is valid if all <see cref="IOpAmp"/>s are operating within
		/// their supply voltage)
		/// </summary>
		private void ConstructInitialAdmittanceMatrix()
		{
			FillPassiveGMatrixDiagonal();
			FillPassiveGMatrixNonDiagonal();

			FillPassiveBMatrix();

			FillPassiveCMatrix();

			FillPassiveDMatrix();

			FillZMatrixCurrents();

			FillZMatrixVoltages();
		}

		/// <summary>
		/// Finds all nodes connected with importnant elements (<see cref="IOpAmp"/>s, <see cref="IVoltageSource"/>s,
		/// <see cref="ICurrentSource"/>s) and stores them in dictionaries for an easy and fast look-up
		/// </summary>
		private void FindImportantNodes()
		{
			// Get the nodes of voltage sources
			_VoltageSourcesNodes = new Dictionary<IVoltageSource, Tuple<int, int>>(_VoltageSources.ToDictionary((source) => source,
				(source) => new Tuple<int, int>(
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(source.TerminalA))),
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(source.TerminalB))))));

			// Get the nodes of current sources
			_CurrentSourcesNodes = new Dictionary<ICurrentSource, Tuple<int, int>>(_CurrentSources.ToDictionary((source) => source,
				(source) => new Tuple<int, int>(
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(source.TerminalA))),
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(source.TerminalB))))));

			// Get the nodes of op-amps
			_OpAmpNodes = new Dictionary<IOpAmp, Tuple<int, int, int>>(_OpAmps.ToDictionary((opAmp) => opAmp,
				(opAmp) => new Tuple<int, int, int>(
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(opAmp.TerminalA))),
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(opAmp.TerminalB))),
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(opAmp.TerminalC))))));
		}

		/// <summary>
		/// Builds the matrix - it's essential to call this method right after constructor
		/// </summary>
		private void Build()
		{
			ConstructNodes();

			ExtractSpecialComponents();

			FindImportantNodes();

			InitializeSubMatrices();

			ConstructInitialAdmittanceMatrix();
		}

		/// <summary>
		/// Finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected terminals), sets their
		/// potential to 0V, removes them from <paramref name="nodes"/>. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		private void ProcessReferenceNodes(List<INode> nodes)
		{
			// Find all reference nodes
			var referenceNodes = FindReferenceNodes(nodes);

			// Set their potential to 0
			referenceNodes.ForEach((node) => node.Potential.Value = 0);

			// Remove them from the nodes list
			nodes.RemoveAll((node) => referenceNodes.Contains(node));
		}

		/// <summary>
		/// Searches through the nodes and finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected
		/// terminals. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns></returns>
		private List<INode> FindReferenceNodes(List<INode> nodes)
		{
			// Filter the nodes, look for all nodes that have na IGround connected to them
			var referenceNodes = new List<INode>(
							nodes.Where((node) => node.ConnectedComponents.Exists((component) => component is IGround)));

			// If any was found, return the list
			if (referenceNodes.Count > 0)
			{
				return referenceNodes;
			}

			// If not, go through all nodes to find a source
			foreach (var node in nodes)
			{
				// Find all sources connected to the node
				var sources = new List<ITwoTerminal>(node.ConnectedComponents.Where((component) =>
					component is IVoltageSource || component is ICurrentSource).Select((source) => source as ITwoTerminal));

				// Go through each source
				foreach (var source in sources)
				{
					// If it's negative (A) terminal is connected to the node, return the node as the reference node
					if (node.ConnectedTerminals.Contains(source.TerminalA))
					{
						return new List<INode>() { node };
					}
				}
			}

			// If no source was found return all nodes as there is no current in the circuit
			return new List<INode>(nodes);
		}

		/// <summary>
		/// Fills the diagonal of a DC admittance matrix - for i-th node adds all admittances connected to it to the admittance
		/// denoted by indexes i,i
		/// </summary>
		private void FillPassiveGMatrixDiagonal()
		{
			// For each node
			for (int i = 0; i < _Nodes.Count; ++i)
			{
				// For each component connected to that node
				_Nodes[i].ConnectedComponents.ForEach((component) =>
				{
					// If the component is a two terminal and imaginary part of its admittance is zero (non-existant)
					if (component is ITwoTerminal twoTerminal && twoTerminal.GetAdmittance(0).Imaginary == 0)
					{
						// Add its admittance to the matrix
						_G[i, i] += twoTerminal.GetAdmittance(0);
					}
				});
			}
		}

		/// <summary>
		/// Fills the non-diagonal entries of a DC admittance matrix - for i,j admittance subtracts from it all admittances located
		/// between node i and node j		
		/// </summary>
		private void FillPassiveGMatrixNonDiagonal()
		{
			// For each node
			for (int i = 0; i < _Nodes.Count; ++i)
			{
				// For each node it's pair with (because of that matrix G is symmetrical so it's only necessary to fill the
				// part below main diagonal and copy the operation to the corresponding entry above the main diagonal
				for (int j = 0; j < i; ++j)
				{
					// Find all components located between node i and node j
					var admittancesBetweenNodesij =
						new List<IBaseComponent>(_Nodes[i].ConnectedComponents.Intersect(_Nodes[j].ConnectedComponents));

					// For each of them
					admittancesBetweenNodesij.ForEach((component) =>
					{
						// If the component is a two terminal and imaginary part of its admittance is zero (non-existant)
						if (component is ITwoTerminal twoTerminal && twoTerminal.GetAdmittance(0).Imaginary == 0)
						{
							// Subtract its admittance to the matrix
							_G[i, j] -= twoTerminal.GetAdmittance(0);

							// And do the same to the entry j,i - admittances between node i,j are identical to admittances
							// between nodes j,i
							_G[j, i] -= twoTerminal.GetAdmittance(0);
						}
					});
				}
			}
		}

		/// <summary>
		/// Fills the B matrix, rules are described in a separate text file
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="voltageSources"></param>
		private void FillPassiveBMatrix()
		{
			// Consider voltage sources, start column is the one on the right of G matrix (whose size is the number of nodes)
			FillBMatrixBasedOnVoltageSources();

			// Consider op-amps, start column is after column related to nodes and voltage sources
			FillBMatrixBasedOnOpAmps();
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveBMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>, fills the
		/// B part of admittance matrix with -1, 0 or 1 based on voltage sources present in the circuit
		/// </summary>
		private void FillBMatrixBasedOnVoltageSources()
		{
			// For every voltage source
			for (int i = 0; i < _VoltageSources.Count; ++i)
			{
				// Get the voltage source's nodes
				var nodes = _VoltageSourcesNodes[_VoltageSources[i]];

				// If the positive terminal is not grounded (TODO: When initial checks for schematics are implemented this isn't necessary
				if (nodes.Item2 != -1)
				{
					// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
					// with 1 (positive terminal)
					_B[nodes.Item2, i] = 1;
				}

				// If the negative terminal is not grounded
				if (nodes.Item1 != -1)
				{
					// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
					// with -1 (negative terminal)
					_B[nodes.Item1, i] = -1;
				}
			}
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveBMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>, fills the
		/// B part of admittance matrix with -1, 0 or 1 based on op-amps present in the circuit (treats their output as a voltage
		/// source)
		/// </summary>
		private void FillBMatrixBasedOnOpAmps()
		{
			// For every op-amp
			for (int i = 0; i < _OpAmps.Count; ++i)
			{
				// If there exists a node to which TerminalC (op-amps output) is connected
				// (it's possible it may not exist due to removed ground node)
				if (_Nodes.Exists((node) => node.ConnectedTerminals.Contains(_OpAmps[i].TerminalC)))
				{
					// Fill the entry in the row corresponding to the node and column corresponding to the source
					// (plus start column) with 1 (positive terminal)
					_B[_Nodes.FindIndex((node) => node.ConnectedTerminals.Contains(_OpAmps[i].TerminalC)),i + _VoltageSources.Count] = 1;
				}
			}
		}

		/// <summary>
		/// Fills the C matrix, rules are described in a separate text file
		/// </summary>
		private void FillPassiveCMatrix()
		{
			// All entries in C are 0 by default

			// Consider voltage sources, the work area in admittance matrix start on the left, below G matrix (size equal to
			// number of nodes)
			FillCMatrixBasedOnVoltageSources();

			// Consider op-amps , the work area in admittance matrix start on the left, below G matrix and rows associated with
			// voltage sources (number of nodes plus number of voltage sources)
			FillCMatrixBasedOnOpAmps();
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveCMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>,
		/// fills C part of admittance matrix with -1, 0 or 1 based on voltage sources present in the circuit
		/// </summary>
		private void FillCMatrixBasedOnVoltageSources()
		{
			// For every voltage source
			for (int i = 0; i < _VoltageSources.Count; ++i)
			{
				// Get the voltage source's nodes
				var nodes = _VoltageSourcesNodes[_VoltageSources[i]];
								
				// If the positive terminal is not grounded (TODO: When initial checks for schematics are implemented this isn't necessary
				if (nodes.Item2 != -1)
				{
					// Fill the entry in the row corresponding to the source (plus starting row)
					// and column corresponding to the node with 1 (positive terminal)
					_C[i, nodes.Item2] = 1;
				}
				
				// If the negative terminal is not grounded
				if (nodes.Item1 != -1)
				{
					// Fill the entry in the row corresponding to the source (plus starting row)
					// and column corresponding to the node with -1 (negative terminal)
					_C[i, nodes.Item1] = -1;
				}
			}
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveCMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>,
		/// fills C part of admittance matrix with -1, 0 or 1 based on op-amps present in the circuit
		/// </summary>
		private void FillCMatrixBasedOnOpAmps()
		{
			// For every op-amp
			for (int i = 0; i < _OpAmps.Count; ++i)
			{
				var opAmp = _OpAmps[i];
				var nodes = _OpAmpNodes[opAmp];

				// Very important: in case the non-inverting input and output are short-circuited (they are the same node) then
				// the value of 1 should be entered into the corresponding cell in _C array. This comes from the fact that having
				// -OpenLoopGain and OpenLoogGain in the _C in the same row enforces potentials at both inputs to be equal. However
				// if the non-inverting input is shorted then the OpenLoopGain value has no place in _C and 1 from the output should
				// be put there.
				// If the inverting input is shorted with the output then the OpenLoopGain should be put int the corresponding cell
				// instead of 1. That's because the initial assumption that V+ = V- is made and if Vout = V- (the same node) then
				// The OpenLoopGain has to be used to guarantee both voltages will be equal
				// (so matrix looks like : ... -k ... k ... | 0 which boils down to kV- = kV+ which means V- = V+)
				// That's why it is very important that if (when) this method is modified the rules presented above are obeyed.

				// If there exists a node to which TerminalA (non-inverting input) is connected
				// (it's possible it may not exist due to removed ground node)
				if (nodes.Item1 != -1)
				{
					// Fill the entry in the row corresponding to the op-amp (plus starting row)
					// and column corresponding to the node (positive terminal) with -OpenLoopGain
					_C[i + _VoltageSources.Count, nodes.Item1] = -opAmp.OpenLoopGain;
				}

				// If there exists a node to which TerminalB (inverting input) is connected
				// (it's possible it may not exist due to removed ground node)
				if (nodes.Item2 != -1)
				{
					// Fill the entry in the row corresponding to the op-amp (plus starting row)
					// and column corresponding to the node (positive terminal) with OpenLoopGain
					_C[i + _VoltageSources.Count, nodes.Item2] = opAmp.OpenLoopGain;
				}

				// If there exists a node to which TerminalB (inverting input) is connected
				// (it's possible it may not exist due to removed ground node)
				// TODO: When initial check of matrix is added the second condition is not necessary (outputs of voltage sources can't
				// be grounded)
				if (nodes.Item3 != nodes.Item2 && nodes.Item3 != -1)
				{
					// Fill the entry in the row corresponding to the op-amp (plus starting row)
					// and column corresponding to the node (positive terminal) with 1 
					_C[i + _VoltageSources.Count, nodes.Item3] = 1;
				}
			}
		}

		/// <summary>
		/// Fills the D matrix according to rules described in a separate text file
		/// </summary>
		private void FillPassiveDMatrix()
		{
			// TODO: Fill accordingly when dependent sources are added
			for (int i = 0; i < _SmallDimension; ++i)
			{
				for (int j = 0; j < _SmallDimension; ++j)
				{
					_D[i, j] = 0;
				}
			}
		}

		/// <summary>
		/// Fills the currents in the Z matrix (the top part)
		/// </summary>
		private void FillZMatrixCurrents()
		{
			// For every node
			for (int i = 0; i < _CurrentSources.Count; ++i)
			{
				// Get the source
				var source = _CurrentSources[i];
				// And its nodes
				var nodes = _CurrentSourcesNodes[source];

				// If the positive terminal is not grounded
				if (nodes.Item2 != -1)
				{
					// Add the produced current to the corresponding entry in _I
					_I[nodes.Item2] += source.ProducedCurrent;
				}

				// If the negative terminal is not grounded
				if (nodes.Item1 != -1)
				{
					// Subtract the produced current from the corresponding entry in _I
					_I[nodes.Item1] -= source.ProducedCurrent;
				}
			}
		}

		/// <summary>
		/// Fills the voltages in the Z matrix (the bottom part)
		/// </summary>
		private void FillZMatrixVoltages()
		{
			// For each voltage source
			for (int i = 0; i < _VoltageSources.Count; ++i)
			{
				// Add its voltge to the i-th entry plus the number of nodes (currents present above in the matrix)
				_E[i] = _VoltageSources[i].ProducedDCVoltage;
			}
		}

		/// <summary>
		/// Checks if <see cref="IOpAmp"/>s operate in the proper region (if they didn't exceed their supply voltages.) If they don't,
		/// adjusts the <see cref="_B"/>, <see cref="_C"/> and <see cref="_E"/> matrices so that, instead of being variable voltage
		/// sources, they are independent voltage sources capped at their supply voltage value. Returns true if an <see cref="IOpAmp"/>
		/// was adjusted and calculations need to be redone, false otherwise
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool CheckOpAmpOperation(Complex[] result)
		{
			for (int i = 0; i < _OpAmps.Count; ++i)
			{
				// The considered op-amp
				var opAmp = _OpAmps[i];
				// Indexes of its nodes
				var nodes = _OpAmpNodes[opAmp];

				// If the output is not grounded TODO: remove the check then the rule that voltage source outputs are not allowed to be
				// grounded is enforced
				if(nodes.Item3 != -1 && (result[nodes.Item3].Real < opAmp.NegativeSupplyVoltage ||
					(result[nodes.Item3].Real > opAmp.PositiveSupplyVoltage)))
				{
					// Op-amp needs adjusting, it's output will now be modeled as an independent voltage source now
					
					// If the non-inverting input is not grounded, reset its entry in the _C matrix
					if (nodes.Item1 != -1)
					{
						_C[_VoltageSources.Count + i, nodes.Item1] = 0;
					}

					// If the inverting input is not grounded, reset its entry in the _C matrix
					if (nodes.Item2 != -1)
					{
						_C[_VoltageSources.Count + i, nodes.Item2] = 0;
					}

					// Set the entry in _B corresponding to the output node to 1
					_B[nodes.Item3, _VoltageSources.Count + i] = 1;

					// And the entry in _C corresponding to the output node to 1
					// It is important that, when non-inverting input is connected directly to the output, the entry in _B
					// corresponding to that node is 1 (and not 0 like the if above would set it). Because this assigning is done after
					// the one for non-inverting input no special conditions are necessary however it's very important to remeber about
					// it if (when) this method is modified
					_C[_VoltageSources.Count + i, nodes.Item3] = 1;

					// Finally, depending on which supply was exceeded, set the value of the source to either positive or negative
					// supply voltage
					_E[_VoltageSources.Count + i] = result[nodes.Item3].Real > opAmp.PositiveSupplyVoltage ?
						opAmp.PositiveSupplyVoltage : opAmp.NegativeSupplyVoltage;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Combines matrices <see cref="_G"/>, <see cref="_B"/>, <see cref="_C"/>, <see cref="_D"/> to create admittance matrix
		/// </summary>
		private Complex[,] ComputeA()
		{
			var result = new Complex[_Size, _Size];

			// _G
			for (int rowIndex = 0; rowIndex < _BigDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _BigDimension; ++columnIndex)
				{
					result[rowIndex, columnIndex] = _G[rowIndex, columnIndex];
				}
			}

			// _B
			for (int rowIndex = 0; rowIndex < _BigDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _SmallDimension; ++columnIndex)
				{
					result[rowIndex, columnIndex + _BigDimension] = _B[rowIndex, columnIndex];
				}
			}

			// _C
			for (int rowIndex = 0; rowIndex < _SmallDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _BigDimension; ++columnIndex)
				{
					result[rowIndex + _BigDimension, columnIndex] = _C[rowIndex, columnIndex];
				}
			}

			// _D
			for (int rowIndex = 0; rowIndex < _SmallDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _SmallDimension; ++columnIndex)
				{
					result[rowIndex + _BigDimension, columnIndex + _BigDimension] = _D[rowIndex, columnIndex];
				}
			}

			return result;
		}

		/// <summary>
		/// Combines matrices <see cref="_I"/> and <see cref="_E"/> to create a vector of free terms for the admittance matrix
		/// </summary>
		/// <returns></returns>
		private Complex[] ComputeZ()
		{
			var result = new Complex[_BigDimension + _SmallDimension];

			// _I
			for (int i = 0; i < _BigDimension; ++i)
			{
				result[i] = _I[i];
			}

			// _E
			for (int i = 0; i < _SmallDimension; ++i)
			{
				result[i + _BigDimension] = _E[i];
			}

			return result;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Solves the matrix for the parameter values present at the moment of calling,
		/// updates the values of nodes and sources currents
		/// </summary>
		public void Solve()
		{
			Complex[] result = null;
			while (true)
			{				
				try
				{
					result = LinearEquations.SimplifiedGaussJordanElimination(ComputeA(), ComputeZ());
				}
				catch (Exception e)
				{
					break;
				}

				if(!CheckOpAmpOperation(result))
				{
					break;
				}
			}

			// Assign the node potentials (entries from 0 to the number of nodes - 1)
			for (int i = 0; i < _BigDimension; ++i)
			{
				_Nodes[i].Potential.Value = result[i].Real;
			}

			// Assign the currents through voltage sources (the remaining entries of the results)
			for (int i = 0; i < _VoltageSources.Count; ++i)
			{
				_VoltageSources[i].ProducedCurrent.Value = result[i + _BigDimension].Real;
			}

		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Constructs and returns an admittance matrix
		/// </summary>
		/// <param name="schematic"></param>
		/// <returns></returns>
		public static AdmittanceMatrix Construct(ISchematic schematic)
		{
			var matrix = new AdmittanceMatrix(schematic);

			matrix.Build();

			return matrix;
		}

		#endregion
	}
}