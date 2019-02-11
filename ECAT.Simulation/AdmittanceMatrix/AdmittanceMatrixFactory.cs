using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Class that allows for easy construction and configuration of admittance matrices for one <see cref="ISchematic"/>
	/// </summary>
	public class AdmittanceMatrixFactory
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public AdmittanceMatrixFactory(ISchematic schematic)
		{
			_Schematic = schematic ?? throw new ArgumentNullException(nameof(schematic));

			Build();
		}

		#endregion

		#region Private properties

		#region Schematic, built matrices, nodes and similar

		/// <summary>
		/// Schematic on which the matrix is based
		/// </summary>
		private ISchematic _Schematic { get; }

		/// <summary>
		/// List with all nodes generated for this <see cref="AdmittanceMatrixDeprecated"/>, order is important - position in the list indicates
		/// the index of the node which directly affects the admittance matrix
		/// </summary>
		private List<INode> _Nodes { get; set; }

		/// <summary>
		/// Node that is used as reference (with potential equal to 0)
		/// </summary>
		private INode _ReferenceNode { get; set; }

		/// <summary>
		/// List with all active components:<see cref="IDCVoltageSource"/>s, <see cref="IACVoltageSource"/>,
		/// <see cref="IOpAmp"/>s
		/// </summary>
		private List<IActiveComponent> _ActiveComponents { get; set; }

		/// <summary>
		/// Dictionary with currents produced by active components (<see cref="_ActiveComponents"/>)
		/// </summary>
		private Dictionary<int, PhasorDomainSignal> _ActiveComponentsCurrents { get; set; }

		#endregion

		#region Collections of components/corresponding nodes

		/// <summary>
		/// List with all DC voltage sources in the <see cref="_Schematic"/>, order is important - position in the list indicates the
		/// index of the source which directly affects the admittance matrix. Does not include op amp outputs
		/// </summary>
		private List<IDCVoltageSource> _DCVoltageSources { get; set; }

		/// <summary>
		/// Dictionary with voltage sources and indexes of their nodes (in order: negative, positive). If a node is grounded
		/// then it is given by -1
		/// </summary>
		private Dictionary<IDCVoltageSource, Tuple<int, int>> _DCVoltageSourcesNodes { get; set; } =
			new Dictionary<IDCVoltageSource, Tuple<int, int>>();

		/// <summary>
		/// List with all AC voltage sources in the <see cref="_Schematic"/>, order is important - position in the list indicates the
		/// index of the source which directly affects the admittance matrix. All sources are also found in the
		/// <see cref="_DCVoltageSources"/> (because an AC voltage source is also always a DC voltage source due to possible DC offset
		/// present in the produced sine)
		/// </summary>
		private List<IACVoltageSource> _ACVoltageSources { get; set; }

		/// <summary>
		/// Dictionary with AC voltage sources and indexes of their nodes (in order: negative, positive). If a node is grounded
		/// then it is given by -1. All nodes are also found in the <see cref="_DCVoltageSourcesNodes"/> (because an AC voltage source
		/// is also always a DC voltage source due to possible DC offset present in the produced sine)
		/// </summary>
		private Dictionary<IACVoltageSource, Tuple<int, int>> _ACVoltageSourcesNodes { get; set; } =
			new Dictionary<IACVoltageSource, Tuple<int, int>>();

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

		#endregion

		#region Common accessors to Counts/Nodes/etc.

		/// <summary>
		/// List with all AC frequencies present in the circuit (DC is present by default and is not included in this list)
		/// </summary>
		private List<double> _FrequenciesInCircuit { get; set; }

		/// <summary>
		/// List with information regarding op-amp outputs - Item1 is the index of the output node, Item2 is the negative supply
		/// and Item3 is the positive supply
		/// </summary>
		private List<Tuple<int, double, double>> _OpAmpOutputs { get; set; } = new List<Tuple<int, double, double>>();

		/// <summary>
		/// Size of A part of the admittance matrix (dependent on nodes)
		/// </summary>
		private int _BigDimension => _Nodes.Count;

		/// <summary>
		/// Size of D part of admittance matrix (depends on the number of independent voltage sources, with op-amp outputs included)
		/// </summary>
		private int _SmallDimension => _TotalVoltageSourcesCount + _OpAmps.Count;

		/// <summary>
		/// Size of the whole admittance matrix
		/// </summary>
		private int _Size => _BigDimension + _SmallDimension;

		/// <summary>
		/// The total number of <see cref="IDCVoltageSource"/>s sources in the <see cref="ISchematic"/>
		/// </summary>
		private int _DCVoltageSourcesCount => _DCVoltageSources.Count;

		/// <summary>
		/// The total number of <see cref="IACVoltageSource"/>s in the <see cref="ISchematic"/>
		/// </summary>
		private int _ACVoltageSourcesCount => _ACVoltageSources.Count;

		/// <summary>
		/// The total number of voltage sources in the <see cref="ISchematic"/>
		/// </summary>
		private int _TotalVoltageSourcesCount => _DCVoltageSourcesCount + _ACVoltageSourcesCount;

		/// <summary>
		/// The total number of <see cref="ICurrentSource"/>s in the <see cref="ISchematic"/>
		/// </summary>
		private int _CurrentSourcesCount => _CurrentSources.Count;

		/// <summary>
		/// True if the circuit is DC only (there are no <see cref="IACVoltageSource"/>s)
		/// </summary>
		private bool _IsPureDC => !_DCVoltageSources.Exists((source) => source is IACVoltageSource);

		/// <summary>
		/// True if the circuit is AC only (there are no <see cref="IOpAmp"/>s, no <see cref="ICurrentSource"/>s and
		/// every <see cref="IDCVoltageSource"/> is an <see cref="IACVoltageSource"/> with <see cref="IDCVoltageSource.ProducedDCVoltage"/>
		/// equal to 0)
		/// </summary>
		private bool _IsPureAC => _OpAmps.Count == 0 && _CurrentSources.Count == 0 &&
			_DCVoltageSources.All((source) => source is IACVoltageSource && source.ProducedDCVoltage == 0);

		/// <summary>
		/// Number of <see cref="IActiveComponent"/>s in the schematic
		/// </summary>
		private int _ActiveComponentsCount => _ActiveComponents.Count;

		#endregion

		#endregion

		#region Public properties

		/// <summary>
		/// Enumeration of all frequencies in the circuit
		/// </summary>
		public IEnumerable<double> FrequenciesInCircuit => _FrequenciesInCircuit;

		/// <summary>
		/// Number of AC voltage sources in the circuit
		/// </summary>
		public int ACVoltageSourcesCount => _ACVoltageSourcesCount;

		/// <summary>
		/// Number of active components in the circuit
		/// </summary>
		public int ActiveComponentsCount => _ActiveComponentsCount;

		/// <summary>
		/// Number of nodes created on basis of <see cref="ISchematic"/> passed to constructor
		/// </summary>
		public int NodesCount => _Nodes.Count;

		#endregion

		#region Private methods

		#region Submatrix initialization

		/// <summary>
		/// Initializes submatrices of the given <see cref="AdmittanceMatrix"/>
		/// </summary>
		/// <param name="matrix"></param>
		private void InitializeSubmatrices(AdmittanceMatrix matrix)
		{
			matrix._A = ArrayHelpers.CreateAndInitialize(Complex.Zero, _BigDimension, _BigDimension);

			matrix._B = ArrayHelpers.CreateAndInitialize<Complex>(0, _BigDimension, _SmallDimension);
			matrix._C = ArrayHelpers.CreateAndInitialize(Complex.Zero, _SmallDimension, _BigDimension);
			matrix._D = ArrayHelpers.CreateAndInitialize(Complex.Zero, _SmallDimension, _SmallDimension);

			matrix._E = ArrayHelpers.CreateAndInitialize(Complex.Zero, _SmallDimension);
			matrix._I = ArrayHelpers.CreateAndInitialize(Complex.Zero, _BigDimension);
		}

		#endregion

		#region Matrix A creation

		/// <summary>
		/// Fills the <see cref="_A"/> Matrix
		/// </summary>
		private void FillAMatrix(double frequency, AdmittanceMatrix matrix)
		{
			FillAMatrixDiagonal(frequency, matrix);

			FillAMatrixNonDiagonal(frequency, matrix);
		}

		/// <summary>
		/// Fills the diagonal of a DC admittance matrix - for i-th node adds all admittances connected to it to the admittance
		/// denoted by indexes i,i
		/// </summary>
		private void FillAMatrixDiagonal(double frequency, AdmittanceMatrix matrix)
		{
			// For each node
			for (int i = 0; i < _Nodes.Count; ++i)
			{
				// For each component connected to that node
				_Nodes[i].ConnectedComponents.ForEach((component) =>
				{
					// If the component is a two terminal
					if (component is ITwoTerminal twoTerminal)
					{
						// Add its admittance to the matrix
						matrix._A[i, i] += twoTerminal.GetAdmittance(frequency);
					}
				});
			}
		}

		/// <summary>
		/// Fills the non-diagonal entries of a DC admittance matrix - for i,j admittance subtracts from it all admittances located
		/// between node i and node j
		/// </summary>
		private void FillAMatrixNonDiagonal(double frequency, AdmittanceMatrix matrix)
		{
			// For each node
			for (int i = 0; i < _Nodes.Count; ++i)
			{
				// For each node it's pair with (because of that matrix A is symmetrical so it's only necessary to fill the
				// part below main diagonal and copy the operation to the corresponding entry above the main diagonal
				for (int j = 0; j < i; ++j)
				{
					// Find all components located between node i and node j
					var admittancesBetweenNodesij =
						new List<IBaseComponent>(_Nodes[i].ConnectedComponents.Intersect(_Nodes[j].ConnectedComponents));

					// For each of them
					admittancesBetweenNodesij.ForEach((component) =>
					{
						// If the component is a two terminal
						if (component is ITwoTerminal twoTerminal)
						{
							// Subtract its admittance to the matrix
							matrix._A[i, j] -= twoTerminal.GetAdmittance(frequency);

							// And do the same to the entry j,i - admittances between node i,j are identical to admittances
							// between nodes j,i
							matrix._A[j, i] -= twoTerminal.GetAdmittance(frequency);
						}
					});
				}
			}
		}

		#endregion

		#region Matrix B creation

		/// <summary>
		/// Helper of <see cref="FillPassiveBMatrix"/>, fills the
		/// B part of admittance matrix with 0 or 1 based on op-amps present in the circuit
		/// </summary>
		private void FillBMatrixOpAmpOutputNodes(AdmittanceMatrix matrix)
		{
			for (int i = 0; i < _OpAmps.Count; ++i)
			{
				// Set the entry in _B corresponding to the output node to 1
				matrix._B[_OpAmpNodes[_OpAmps[i]].Item3, _TotalVoltageSourcesCount + i] = 1;
			}
		}

		#endregion

		#region Op-amp operation mode switching

		/// <summary>
		/// Configures submatrices so that <paramref name="opAmp"/> is considered to work in active operation (output is between
		/// supply voltages)
		/// </summary>
		/// <param name="opAmp"></param>
		private void ConfigureForActiveOperation(AdmittanceMatrix matrix, int opAmpIndex)
		{
			// Get the index of the op-amp
			var opAmp = _OpAmps[opAmpIndex];

			// Indexes of its nodes
			var nodes = _OpAmpNodes[opAmp];

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//																													 //
			// Very important: in case the non-inverting input and output are short-circuited (they are the same node) then		 //
			// the value of 1 should be entered into the corresponding cell in _C array. This comes from the fact that having	 //
			// -OpenLoopGain and OpenLoogGain in the _C in the same row enforces potentials at both inputs to be equal. However  //
			// if the non-inverting input is shorted then the OpenLoopGain value has no place in _C and 1 from the output should //
			// be put there.																									 //
			// If the inverting input is shorted with the output then the OpenLoopGain should be put int the corresponding cell  //
			// instead of 1. That's because the initial assumption that V+ = V- is made and if Vout = V- (the same node) then	 //
			// The OpenLoopGain has to be used to guarantee both voltages will be equal											 //
			// (so matrix looks like : ... -k ... k ... | 0 which boils down to kV- = kV+ which means V- = V+)					 //
			// That's why it is very important that if (when) this method is modified the rules presented above are obeyed.		 //
			//																													 //
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			// If there exists a node to which TerminalA (non-inverting input) is connected
			// (it's possible it may not exist due to removed ground node)
			if (nodes.Item1 != -1)
			{
				// Fill the entry in the row corresponding to the op-amp (plus starting row)
				// and column corresponding to the node (positive terminal) with -OpenLoopGain
				matrix._C[_TotalVoltageSourcesCount + opAmpIndex, nodes.Item1] = -opAmp.OpenLoopGain;
			}

			// If there exists a node to which TerminalB (inverting input) is connected
			// (it's possible it may not exist due to removed ground node)
			if (nodes.Item2 != -1)
			{
				// Fill the entry in the row corresponding to the op-amp (plus starting row)
				// and column corresponding to the node (positive terminal) with OpenLoopGain
				matrix._C[_TotalVoltageSourcesCount + opAmpIndex, nodes.Item2] = opAmp.OpenLoopGain;
			}

			// If the output is not shorted with the inverting input
			if (nodes.Item3 != nodes.Item2)
			{
				matrix._C[_TotalVoltageSourcesCount + opAmpIndex, nodes.Item3] = 1;
			}

			// Fill the entry in the row corresponding to the op-amp (plus starting row)
			// and column corresponding to the node (positive terminal) with 1 
			matrix._E[_TotalVoltageSourcesCount + opAmpIndex] = 0;
		}

		/// <summary>
		/// Configures submatrices so that <paramref name="opAmp"/> is considered to work in active operation (output is between
		/// supply voltages)
		/// </summary>
		/// <param name="opAmp"></param>
		/// <param name="positiveSaturation">If true, the output is set to positive supply voltage, if false to the negative
		/// supply</param>
		private void ConfigureForSaturation(AdmittanceMatrix matrix, int opAmpIndex, bool positiveSaturation)
		{
			// Get the index of the op-amp
			var opAmp = _OpAmps[opAmpIndex];
			// Indexes of its nodes
			var nodes = _OpAmpNodes[opAmp];

			// Op-amp needs adjusting, its output will now be modeled as an independent voltage source now

			// If the non-inverting input is not grounded, reset its entry in the _C matrix
			if (nodes.Item1 != -1)
			{
				matrix._C[_TotalVoltageSourcesCount + opAmpIndex, nodes.Item1] = 0;
			}

			// If the inverting input is not grounded, reset its entry in the _C matrix
			if (nodes.Item2 != -1)
			{
				matrix._C[_TotalVoltageSourcesCount + opAmpIndex, nodes.Item2] = 0;
			}

			// And the entry in _C corresponding to the output node to 1
			// It is important that, when non-inverting input is connected directly to the output, the entry in _B
			// corresponding to that node is 1 (and not 0 like the if above would set it). Because this assigning is done after
			// the one for non-inverting input no special conditions are necessary however it's very important to remeber about
			// it if (when) this method is modified
			matrix._C[_TotalVoltageSourcesCount + opAmpIndex, nodes.Item3] = 1;

			// Finally, depending on which supply was exceeded, set the value of the source to either positive or negative
			// supply voltage
			matrix._E[_TotalVoltageSourcesCount + opAmpIndex] = positiveSaturation ? opAmp.PositiveSupplyVoltage : opAmp.NegativeSupplyVoltage;
		}

		#endregion

		#region DC voltage source activation

		/// <summary>
		/// Activates all DC voltage sources
		/// </summary>
		private void ConfigureDCVoltageSources(AdmittanceMatrix matrix, bool state)
		{
			for (int i = 0; i < _DCVoltageSourcesCount; ++i)
			{
				ConfigureDCVoltageSource(matrix, i, state);
			}
		}

		/// <summary>
		/// Activates all DC voltage sources
		/// </summary>
		private void ConfigureACVoltageSourcesForDC(AdmittanceMatrix matrix, bool state)
		{
			for (int i = 0; i < ACVoltageSourcesCount; ++i)
			{
				ActivateACVoltageSourceForDC(matrix, i, state);
			}
		}

		/// <summary>
		/// Activates the DC voltage sources given by the index
		/// </summary>
		/// <param name="sourceIndex"></param>
		/// <param name="state">True if the source is active, false if not (it is considered as short-circuit)</param>
		private void ConfigureDCVoltageSource(AdmittanceMatrix matrix, int sourceIndex, bool state)
		{
			// Get the voltage source's nodes
			var nodes = _DCVoltageSourcesNodes[_DCVoltageSources[sourceIndex]];

			// If the positive terminal is not grounded
			if (nodes.Item2 != -1)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with 1 (positive terminal)
				matrix._B[nodes.Item2, sourceIndex] = 1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with 1 (positive terminal)
				matrix._C[sourceIndex, nodes.Item2] = 1;
			}

			// If the negative terminal is not grounded
			if (nodes.Item1 != -1)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with -1 (negative terminal)
				matrix._B[nodes.Item1, sourceIndex] = -1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with -1 (negative terminal)
				matrix._C[sourceIndex, nodes.Item1] = -1;
			}

			if (state)
			{
				matrix._E[sourceIndex] = _DCVoltageSources[sourceIndex].ProducedDCVoltage;
			}
		}

		/// <summary>
		/// Activates the DC voltage sources given by the index
		/// </summary>
		/// <param name="sourceIndex"></param>
		/// <param name="state">True if the source is active, false if not (it is considered as short-circuit)</param>
		private void ActivateACVoltageSourceForDC(AdmittanceMatrix matrix, int sourceIndex, bool state)
		{
			// Get the voltage source's nodes
			var nodes = _ACVoltageSourcesNodes[_ACVoltageSources[sourceIndex]];

			// If the positive terminal is not grounded
			if (nodes.Item2 != -1)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with 1 (positive terminal)
				matrix._B[nodes.Item2, sourceIndex + _DCVoltageSources.Count] = 1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with 1 (positive terminal)
				matrix._C[sourceIndex + _DCVoltageSources.Count, nodes.Item2] = 1;
			}

			// If the negative terminal is not grounded
			if (nodes.Item1 != -1)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with -1 (negative terminal)
				matrix._B[nodes.Item1, sourceIndex + _DCVoltageSources.Count] = -1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with -1 (negative terminal)
				matrix._C[sourceIndex + _DCVoltageSources.Count, nodes.Item1] = -1;
			}

			if (state)
			{
				matrix._E[sourceIndex + _DCVoltageSources.Count] = _ACVoltageSources[sourceIndex].ProducedDCVoltage;
			}
		}

		#endregion

		#region AC voltage source activation

		/// <summary>
		/// Activates the AC voltage source given by the index
		/// </summary>
		/// <param name="sourceIndex"></param>
		/// <param name="state">True if the source is active, false if not (it is considered as short-circuit)</param>
		private void ConfigureACVoltageSource(AdmittanceMatrix matrix, int sourceIndex, bool state)
		{
			// Get the voltage source's nodes
			var nodes = _ACVoltageSourcesNodes[_ACVoltageSources[sourceIndex]];

			// If the positive terminal is not grounded
			if (nodes.Item2 != -1)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with 1 (positive terminal)
				matrix._B[nodes.Item2, sourceIndex + _DCVoltageSources.Count] = 1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with 1 (positive terminal)
				matrix._C[sourceIndex + _DCVoltageSources.Count, nodes.Item2] = 1;
			}

			// If the negative terminal is not grounded
			if (nodes.Item1 != -1)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with -1 (negative terminal)
				matrix._B[nodes.Item1, sourceIndex + _DCVoltageSources.Count] = -1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with -1 (negative terminal)
				matrix._C[sourceIndex + _DCVoltageSources.Count, nodes.Item1] = -1;
			}
			
			matrix._E[sourceIndex + _DCVoltageSources.Count] = state ? 1 : 0;
		}

		#endregion

		#region Current source activation

		/// <summary>
		/// Activates all DC voltage sources
		/// </summary>
		private void ActivateCurrentSources(AdmittanceMatrix matrix)
		{
			for (int i = 0; i < _CurrentSourcesCount; ++i)
			{
				ActivateCurrentSource(matrix, i);
			}
		}

		/// <summary>
		/// Activates the current source given by the index
		/// </summary>
		/// <param name="sourceIndex"></param>
		private void ActivateCurrentSource(AdmittanceMatrix matrix, int sourceIndex)
		{
			// Get the nodes
			var nodes = _CurrentSourcesNodes[_CurrentSources[sourceIndex]];

			// If the negative terminal is not grounded
			if (nodes.Item1 != -1)
			{
				// Subtract source's current from the node
				matrix._I[nodes.Item1] -= _CurrentSources[sourceIndex].ProducedCurrent;
			}

			// If the positive terminal is not grounded
			if (nodes.Item2 != -1)
			{
				// Add source's current to the node
				matrix._I[nodes.Item2] += _CurrentSources[sourceIndex].ProducedCurrent;
			}
		}

		/// <summary>
		/// Builds the matrix - it's essential to call this method right after constructor
		/// </summary>
		private void Build()
		{
			ConstructNodes();

			ExtractSpecialComponents();

			FindFrequenciesInCircuit();

			FindImportantNodes();

			CheckOpAmpOutputs();

			GenerateOpAmpOutputInformation();
		}

		#endregion

		#region Node generation

		/// <summary>
		/// Helper of <see cref="Build"/>, constructs nodes, sets all potentials to 0, finds and removes reference nodes, assigns
		/// indexes
		/// </summary>
		private void ConstructNodes()
		{
			// Generate nodes using helper class
			_Nodes = NodeGenerator.Generate(_Schematic);

			// Find, assign and remove the reference (ground) nodes
			ProcessReferenceNodes();

			// Assign indexes to nodes
			for (int i = 0; i < _Nodes.Count; ++i)
			{
				// Node indexing starts at ground node index + 1
				_Nodes[i].Index = SimulationManager.GroundNodeIndex + 1 + i;
			}
		}

		/// <summary>
		/// Finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected terminals), sets their
		/// potential to 0V, removes them from <paramref name="nodes"/>. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		private void ProcessReferenceNodes()
		{
			// Find all reference nodes
			var referenceNodes = FindReferenceNodes();

			// Assign the ground index to the node
			referenceNodes.ForEach((node) => node.Index = SimulationManager.GroundNodeIndex);

			// Remove them from the nodes list
			_Nodes.RemoveAll((node) => referenceNodes.Contains(node));

			// Create a new reference node
			_ReferenceNode = new Node();

			// Merge every node that was determined to be a reference node with it
			referenceNodes.ForEach((node) => _ReferenceNode.Merge(node));
		}

		/// <summary>
		/// Searches through the nodes and finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected
		/// terminals. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns></returns>
		private List<INode> FindReferenceNodes()
		{
			// Filter the nodes, look for all nodes that have na IGround connected to them
			var referenceNodes = new List<INode>(
							_Nodes.Where((node) => node.ConnectedComponents.Exists((component) => component is IGround)));

			// If any was found, return the list
			if (referenceNodes.Count > 0)
			{
				return referenceNodes;
			}

			// If not, go through all nodes to find a source
			foreach (var node in _Nodes)
			{
				// Find all sources connected to the node
				var sources = new List<ITwoTerminal>(node.ConnectedComponents.Where((component) =>
					component is IDCVoltageSource || component is ICurrentSource).Select((source) => source as ITwoTerminal));

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
			return new List<INode>(_Nodes);
		}

		#endregion

		#region Initialization of private collections

		/// <summary>
		/// Extracts all frequencies from the circuit by checking what's the frequency of every <see cref="IACVoltageSource"/>
		/// </summary>
		private void FindFrequenciesInCircuit() =>
			_FrequenciesInCircuit = new List<double>(_ACVoltageSources.Select((source) => source.Frequency).Distinct());

		/// <summary>
		/// Initializes collections related with <see cref="IActiveComponent"/>s
		/// </summary>
		private void InitializeActiveComponents()
		{
			// Create a list with active components
			_ActiveComponents = new List<IActiveComponent>(
				_Schematic.Components.Where((component) => component is IActiveComponent).Cast<IActiveComponent>());

			// Create a dictionary for their currents
			_ActiveComponentsCurrents = new Dictionary<int, PhasorDomainSignal>();

			// Assign index to each active component and add the component to the ActiveComponentsDictionary
			for (int i = 0; i < _ActiveComponents.Count; ++i)
			{
				_ActiveComponents[i].ActiveComponentIndex = i;
				_ActiveComponentsCurrents.Add(i, new PhasorDomainSignal());
			}
		}

		/// <summary>
		/// Extracts all components that require special care (<see cref="IDCVoltageSource"/>s, <see cref="ICurrentSource"/>s,
		/// <see cref="IOpAmp"/>s) to their respective containers
		/// </summary>
		private void ExtractSpecialComponents()
		{
			// Get the voltage sources
			_DCVoltageSources = new List<IDCVoltageSource>(_Schematic.Components.
				Where((component) => component is IDCVoltageSource).
				Cast<IDCVoltageSource>());

			// Get the AC voltage sources
			_ACVoltageSources = new List<IACVoltageSource>(_Schematic.Components.
				Where((component) => component is IACVoltageSource).
				Cast<IACVoltageSource>()); ;

			// Get the current sources
			_CurrentSources = new List<ICurrentSource>(_Schematic.Components.Where((component) => component is ICurrentSource).
				Cast<ICurrentSource>());

			// Get the op-amps
			_OpAmps = new List<IOpAmp>(_Schematic.Components.Where((component) => component is IOpAmp).Cast<IOpAmp>());

			// Prepare the active components
			InitializeActiveComponents();
		}

		/// <summary>
		/// Finds all nodes connected with importnant elements (<see cref="IOpAmp"/>s, <see cref="IDCVoltageSource"/>s,
		/// <see cref="ICurrentSource"/>s) and stores them in dictionaries for an easy and fast look-up
		/// </summary>
		private void FindImportantNodes()
		{
			// Get the nodes of voltage sources
			_DCVoltageSourcesNodes = new Dictionary<IDCVoltageSource, Tuple<int, int>>(_DCVoltageSources.ToDictionary((source) => source,
				(source) => new Tuple<int, int>(
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(source.TerminalA))),
				_Nodes.IndexOf(_Nodes.Find((node) => node.ConnectedTerminals.Contains(source.TerminalB))))));

			// Get the nodes of the AC voltage sources
			_ACVoltageSourcesNodes = new Dictionary<IACVoltageSource, Tuple<int, int>>(_ACVoltageSources.ToDictionary((source) => source,
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
		/// Generates information about limitations on op-amp outputs imposed by their supply voltages
		/// </summary>
		private void GenerateOpAmpOutputInformation() =>
			_OpAmpOutputs = new List<Tuple<int, double, double>>(_OpAmps.Select((opAmp) => new Tuple<int, double, double>(
			_OpAmpNodes[opAmp].Item3, opAmp.NegativeSupplyVoltage, opAmp.PositiveSupplyVoltage)));

		#endregion

		#region Initial OpAmp configuration

		/// <summary>
		/// Constructs the intial version of the admittance matrix (which is valid if all <see cref="IOpAmp"/>s are operating within
		/// their supply voltage)
		/// </summary>
		private void InitialOpAmpSettings(AdmittanceMatrix matrix)
		{
			FillBMatrixOpAmpOutputNodes(matrix);

			// Configure each op-amp for active operation (by default)
			for (int i = 0; i < _OpAmps.Count; ++i)
			{
				ConfigureForActiveOperation(matrix, i);
			}
		}
		
		#endregion

		#region Correctness checks

		/// <summary>
		/// Checks if all <see cref="IOpAmp"/> have their outputs not grounded. If such thing occurs an exception is thrown
		/// </summary>
		private void CheckOpAmpOutputs()
		{
			foreach (var item in _OpAmpNodes)
			{
				if (item.Value.Item3 == -1)
				{
					throw new Exception("Output of a(n) " + IoC.Resolve<IComponentFactory>().GetDeclaration<IOpAmp>().DisplayName +
						" cannot be grounded");
				}
			}
		}

		#endregion

		#endregion

		#region Public methods

		/// <summary>
		/// Constructs a DC addmittance matrix. It is constructed for all DC voltage sources, DC current sources and DC offsets of AC voltage sources
		/// turned on. Each potential resulting from the solution of this matrix is the voltage at corresponding node.
		/// </summary>
		/// <returns></returns>
		public AdmittanceMatrix ConstructDC()
		{
			var matrix = new AdmittanceMatrix(_BigDimension, _SmallDimension);

			InitializeSubmatrices(matrix);

			// TODO: Short circuit inductors

			// Fill A matrix, which is dependent on admittances between nodes
			FillAMatrix(0, matrix);
	
			// Initialize op-amp settings - active operation
			InitialOpAmpSettings(matrix);

			// Turn on DC voltage sources (in admittance matrix it is represented by Ua = Ub)
			ConfigureDCVoltageSources(matrix, true);

			// Turn on DC current sources
			ActivateCurrentSources(matrix);

			// Configure AC voltage sources for their DC offset
			ConfigureACVoltageSourcesForDC(matrix, true);

			return matrix;
		}

		/// <summary>
		/// Returns an AC admittance matrix for voltage source given by <paramref name="voltageSourceIndex"/>.
		/// The source is regarded as a unity source so that any node potential is in fact the transfer function and can be used to determine
		/// potentials for arbitrary voltage source values (Unode = K(jw) * Usource, for Usource = 1 we have Unode = K(jw) which allows to
		/// obtain K(jw) which can be then substituted to Unode = K(jw) * Usource to obtain Unode for any Usource).
		/// </summary>
		/// <param name="voltageSourceIndex"></param>
		/// <returns></returns>
		public AdmittanceMatrix ConstructAC(int voltageSourceIndex)
		{
			var matrix = new AdmittanceMatrix(_BigDimension, _SmallDimension);

			InitializeSubmatrices(matrix);
							
			// Fill A matrix, which is dependent on admittances between nodes
			FillAMatrix(_ACVoltageSources[voltageSourceIndex].Frequency, matrix);

			// Initialize op-amp settings - active operation
			InitialOpAmpSettings(matrix);

			// Configure DC voltage sources to be off (in admittance matrix it is represented by Ua = Ub; two node potentials are equal)
			ConfigureDCVoltageSources(matrix, false);
			
			// Turn off all AC voltage sources except the chosen one
			for(int i = 0; i < _ACVoltageSourcesCount; ++i)
			{
				ConfigureACVoltageSource(matrix, i, i == voltageSourceIndex);
			}

			return matrix;
		}

		/// <summary>
		/// Returns all nodes generated for the <see cref="ISchematic"/> passed to constructor
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INode> GetNodes() => _Nodes.ConcatAtBeginning(_ReferenceNode);

		/// <summary>
		/// Returns frequency of AC voltage source given by <paramref name="sourceIndex"/>, throws an exception if the index is equal to or greater
		/// than the number of AC voltage sources. Indexing starts at 0.
		/// </summary>
		/// <param name="sourceIndex"></param>
		/// <returns></returns>
		public double GetACVoltageSourceFrequency(int sourceIndex) => sourceIndex < _ACVoltageSourcesCount ? _ACVoltageSources[sourceIndex].Frequency :
			throw new ArgumentOutOfRangeException(nameof(sourceIndex));

		/// <summary>
		/// Returns amplitude of AC voltage source given by <paramref name="sourceIndex"/>, throws an exception if the index is equal to or greater
		/// than the number of AC voltage sources. Indexing starts at 0.
		/// </summary>
		/// <param name="sourceIndex"></param>
		/// <returns></returns>
		public double GetACVoltageSourceAmplitude(int sourceIndex) => sourceIndex < _ACVoltageSourcesCount ?
			_ACVoltageSources[sourceIndex].PeakProducedVoltage :	throw new ArgumentOutOfRangeException(nameof(sourceIndex));

		#endregion
	}
}