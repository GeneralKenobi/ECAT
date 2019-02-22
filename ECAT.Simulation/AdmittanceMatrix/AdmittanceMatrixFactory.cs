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

		#region Schematic and nodes

		/// <summary>
		/// Schematic on which the factory is based
		/// </summary>
		private ISchematic _Schematic { get; }

		/// <summary>
		/// List with all nodes, except reference node, generated for this <see cref="AdmittanceMatrixDeprecated"/>, order is important - position
		/// in the list indicates the index of the node which directly affects the admittance matrix
		/// </summary>
		private List<INode> _Nodes { get; set; }

		/// <summary>
		/// Node that is used as reference (with potential equal to 0)
		/// </summary>
		private INode _ReferenceNode { get; set; }

		#endregion

		#region Collections of components/corresponding nodes

		/// <summary>
		/// List with descriptions of all DC voltage sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ISourceDescription> _DCVoltageSources { get; set; }

		/// <summary>
		/// List with descriptions of all AC voltage sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ISourceDescription> _ACVoltageSources { get; set; }

		/// <summary>
		/// List with descriptions of all DC current sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ISourceDescription> _DCCurrentSources { get; set; }

		/// <summary>
		/// List with descriptions of all op-amps in the <see cref="_Schematic"/>
		/// </summary>
		private IList<IOpAmpDescription> _OpAmps { get; set; }

		/// <summary>
		/// Dictionary mapping components (<see cref="IComponentDescription"/>s representing them) to indices.
		/// Components mapped: <see cref="IDCVoltageSource"/>s, <see cref="IACVoltageSource"/>s, <see cref="ICurrentSource"/>s,
		/// <see cref="IOpAmp"/>s.
		/// </summary>
		private IDictionary<IComponentDescription, int> _IndexedComponentsIndices { get; set; }
		
		/// <summary>
		/// Dictionary with indices of nodes of <see cref="IACVoltageSource"/>s, <see cref="IDCVoltageSource"/>s and <see cref="ICurrentSource"/>s
		/// </summary>
		private IDictionary<ISourceDescription, TwoTerminalSourceNodeInfo> _SourcesNodes { get; set; }

		/// <summary>
		/// Dictionary with indices of nodes of <see cref="IOpAmp"/>s
		/// </summary>
		private IDictionary<IOpAmpDescription, OpAmpNodeInfo> _OpAmpsNodes { get; set; }

		/// <summary>
		/// Dictionary containing operation mode assigned to each op-amp.
		/// </summary>
		private IDictionary<IOpAmpDescription, OpAmpOperationMode> _OpAmpOperation { get; } = new Dictionary<IOpAmpDescription, OpAmpOperationMode>();

		#endregion

		#region Admittance matrix dimensions

		/// <summary>
		/// Size of A part of the admittance matrix (equal to number of nodes)
		/// </summary>
		private int _BigDimension => _Nodes.Count;

		/// <summary>
		/// Size of D part of admittance matrix (equal to number of independent voltage sources, with op-amp outputs included)
		/// </summary>
		private int _SmallDimension => _SourcesNodes.Count;

		#endregion

		#endregion

		#region Public properties

		/// <summary>
		/// Number of nodes created on basis of <see cref="ISchematic"/> passed to constructor
		/// </summary>
		public int NodesCount => _Nodes.Count;

		/// <summary>
		/// Lowest frequency in the circuit
		/// </summary>
		public double LowestFrequency { get; private set; }

		/// <summary>
		/// Highest frequency in the circuit
		/// </summary>
		public double HighestFrequency { get; private set; }

		/// <summary>
		/// Number of <see cref="IACVoltageSource"/>s in the circuit
		/// </summary>
		public int ACVoltageSourcesCount => _ACVoltageSources.Count;

		/// <summary>
		/// Number of <see cref="IDCVoltageSource"/>s in the circuit
		/// </summary>
		public int DCVoltageSourcesCount => _DCVoltageSources.Count;

		/// <summary>
		/// Number of <see cref="ICurrentSource"/>s in the circuit
		/// </summary>
		public int DCCurrentSourcesCount => _DCCurrentSources.Count;

		/// <summary>
		/// Number of <see cref="IOpAmp"/>s in the circuit
		/// </summary>
		public int OpAmpsCount => _OpAmps.Count;

		/// <summary>
		/// Number of active components in the circuit
		/// </summary>
		public int ActiveComponentsCount { get; private set; }

		/// <summary>
		/// Returns descriptions of AC voltage sources
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISourceDescription> ACVoltageSources => _ACVoltageSources;

		/// <summary>
		/// Returns descriptions of DC sources
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISourceDescription> DCVoltageSources => _DCVoltageSources;

		/// <summary>
		/// Returns descriptions of DC sources
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISourceDescription> DCCurrentSources => _DCCurrentSources;

		#endregion

		#region Private methods

		#region Initialization

		/// <summary>
		/// Builds the factory - it's essential to call this method right after constructor
		/// </summary>
		private void Build()
		{
			ConstructNodes();

			ExtractSpecialComponents();

			FindFrequenciesInCircuit();

			FindImportantNodes();

			CheckOpAmpOutputs();

			InitializeOpAmpOperationCollection();
		}

		#endregion

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
		private void FillAMatrix(AdmittanceMatrix matrix, double frequency)
		{
			FillAMatrixDiagonal(matrix, frequency);

			FillAMatrixNonDiagonal(matrix, frequency);
		}

		/// <summary>
		/// Fills the diagonal of an admittance matrix - for i-th node adds all admittances connected to it, to the cell denoted by indices i,i
		/// </summary>
		private void FillAMatrixDiagonal(AdmittanceMatrix matrix, double frequency)
		{
			// For each node
			foreach(var node in _Nodes)
			{
				// For each component connected to that node
				node.ConnectedComponents.ForEach((component) =>
				{
					// If the component is a two terminal
					if (component is ITwoTerminal twoTerminal)
					{
						// Add its admittance to the matrix
						matrix._A[node.Index, node.Index] += twoTerminal.GetAdmittance(frequency);
					}
				});
			}
		}
		
		/// <summary>
		/// Fills the non-diagonal entries of an admittance matrix - for entry i,j all admittances located between node i and node j are subtracted
		/// from that entry
		/// </summary>
		private void FillAMatrixNonDiagonal(AdmittanceMatrix matrix, double frequency)
		{
			// For each node
			foreach(var node1 in _Nodes)
			{
				// Matrix A is symmetrical along the main diagonal so it's only necessary to fill the
				// part below main diagonal and copy the operation to the corresponding entry above the main diagonal
				foreach(var node2 in _Nodes.FindAll((x) => x.Index < node1.Index))
				{
					// Find all components located between node i and node j
					var admittancesBetweenNodesij =
						new List<IBaseComponent>(node1.ConnectedComponents.Intersect(node2.ConnectedComponents));

					// For each of them
					admittancesBetweenNodesij.ForEach((component) =>
					{
						// If the component is a two terminal
						if (component is ITwoTerminal twoTerminal)
						{
							// Get the admittance of the element
							var admittance = twoTerminal.GetAdmittance(frequency);

							// Subtract its admittance from the matrix
							matrix._A[node1.Index, node2.Index] -= admittance;

							// And do the same to the entry j,i - admittances between node i,j are identical to admittances between nodes j,i
							matrix._A[node2.Index, node1.Index] -= admittance;
						}
					});
				}
			}
		}

		#endregion

		#region Matrix B creation

		/// <summary>
		/// Fills the B part of admittance matrix with 0 or 1 based on op-amps present in the circuit
		/// </summary>
		private void FillBMatrixOpAmpOutputNodes(AdmittanceMatrix matrix)
		{
			// For each op-amp
			foreach(var opAmp in _OpAmps)
			{
				// Set the entry in _B corresponding to the output node to 1
				matrix._B[_OpAmpsNodes[opAmp].Output, _IndexedComponentsIndices[opAmp]] = 1;
			}
		}

		#endregion

		#region Op-amp operation mode switching
		
		/// <summary>
		/// Configures <see cref="IOpAmp"/> for operation in <paramref name="operationMode"/> in <paramref name="matrix"/>.
		/// Saturation mode will generate output voltage only if matrix is built for DC (<paramref name="ac"/> is false).
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="opAmpIndex"></param>
		/// <param name="operationMode"></param>
		private void ConfigureOpAmpOperation(AdmittanceMatrix matrix, IOpAmpDescription opAmp, OpAmpOperationMode operationMode, bool ac)
		{
			// Call appropriate method based on operation mode
			switch (_OpAmpOperation[opAmp])
			{
				case OpAmpOperationMode.Active:
					{
						ConfigureOpAmpForActiveOperation(matrix, opAmp);
					}
					break;

				case OpAmpOperationMode.PositiveSaturation:
					{
						ConfigureOpAmpForSaturation(matrix, opAmp, true, ac);
					}
					break;

				case OpAmpOperationMode.NegativeSaturation:
					{
						ConfigureOpAmpForSaturation(matrix, opAmp, false, ac);
					}
					break;

				default:
					{
						throw new Exception("Unhandled case");
					}
			}
		}

		/// <summary>
		/// Configures submatrices so that <paramref name="opAmp"/> is considered to work in active operation (output is between
		/// supply voltages)
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="opAmp"></param>
		private void ConfigureOpAmpForActiveOperation(AdmittanceMatrix matrix, IOpAmpDescription opAmp)
		{
			// Get nodes of the op-amp
			var nodes = _OpAmpsNodes[opAmp];

			// As well as its index
			var index = _IndexedComponentsIndices[opAmp];

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
			if (nodes.NonInvertingInput != GroundNodeIndex)
			{
				// Fill the entry in the row corresponding to the op-amp (plus starting row)
				// and column corresponding to the node (positive terminal) with -OpenLoopGain
				matrix._C[index, nodes.NonInvertingInput] = -opAmp.OpenLoopGain;
			}

			// If there exists a node to which TerminalB (inverting input) is connected
			// (it's possible it may not exist due to removed ground node)
			if (nodes.InvertingInput != GroundNodeIndex)
			{
				// Fill the entry in the row corresponding to the op-amp (plus starting row)
				// and column corresponding to the node (positive terminal) with OpenLoopGain
				matrix._C[index, nodes.InvertingInput] = opAmp.OpenLoopGain;
			}

			// If the output is not shorted with the inverting input
			if (nodes.Output != nodes.InvertingInput)
			{
				matrix._C[index, nodes.Output] = 1;
			}

			// Fill the entry in the row corresponding to the op-amp (plus starting row)
			// and column corresponding to the node (positive terminal) with 1 
			matrix._E[index] = 0;
		}

		/// <summary>
		/// Configures submatrices so that <paramref name="opAmp"/> is considered to work in saturatio (output is either positive or negative
		/// supply voltage - depending on <paramref name="positiveSaturation"/>)
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="opAmp"></param>
		/// <param name="positiveSaturation">If true, the output is set to positive supply voltage, if false to the negative
		/// supply</param>
		/// <param name="ac"></param>
		private void ConfigureOpAmpForSaturation(AdmittanceMatrix matrix, IOpAmpDescription opAmp, bool positiveSaturation, bool ac)
		{
			// Indices of op-amps nodes
			var nodes = _OpAmpsNodes[opAmp];

			// And its index
			var index = _IndexedComponentsIndices[opAmp];

			// If the non-inverting input is not grounded, reset its entry in the _C matrix
			if (nodes.NonInvertingInput != GroundNodeIndex)
			{
				matrix._C[index, nodes.NonInvertingInput] = 0;
			}

			// If the inverting input is not grounded, reset its entry in the _C matrix
			if (nodes.InvertingInput != GroundNodeIndex)
			{
				matrix._C[index, nodes.InvertingInput] = 0;
			}

			// And the entry in _C corresponding to the output node to 1
			// It is important that, when non-inverting input is connected directly to the output, the entry in _B
			// corresponding to that node is 1 (and not 0 like the if above would set it). Because this assigning is done after
			// the one for non-inverting input no special conditions are necessary however it's very important to remeber about
			// it if (when) this method is modified
			matrix._C[index, nodes.Output] = 1;

			// Finally, depending on which supply was exceeded, set the value of the source to either positive or negative
			// supply voltage
			if (!ac)
			{
				matrix._E[index] = positiveSaturation ? opAmp.PositiveSupplyVoltage: opAmp.NegativeSupplyVoltage;
			}
		}

		#endregion

		#region Voltage source activation

		/// <summary>
		/// Configures all voltage sources for specific operation. Outputs are set to 1 in order to generate transfer functions.
		/// </summary>
		private void ConfigureVoltageSources(AdmittanceMatrix matrix, bool state)
		{
			// Take all voltage sources (DC + AC)
			foreach(var source in _DCVoltageSources.Concat(_ACVoltageSources))
			{
				// And configure them for the state
				ConfigureVoltageSource(matrix, source, state);
			}
		}

		/// <summary>
		/// Configures the <paramref name="source"/> for desired <paramref name="state"/>.
		/// Outputs is set to 1 in order to generate transfer functions.
		/// </summary>
		/// <param name="sourceIndex"></param>
		/// <param name="state">True if the source is active, false if not (it is considered as short-circuit)</param>
		private void ConfigureVoltageSource(AdmittanceMatrix matrix, ISourceDescription source, bool state)
		{
			// Get the voltage source's nodes
			var nodes = _SourcesNodes[source];

			// And its index
			var sourceIndex = _IndexedComponentsIndices[source];

			// If the positive terminal is not grounded
			if (nodes.Positive != GroundNodeIndex)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with 1 (positive terminal)
				matrix._B[nodes.Positive, sourceIndex] = 1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with 1 (positive terminal)
				matrix._C[sourceIndex, nodes.Positive] = 1;
			}

			// If the negative terminal is not grounded
			if (nodes.Negative != GroundNodeIndex)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with -1 (negative terminal)
				matrix._B[nodes.Negative, sourceIndex] = -1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with -1 (negative terminal)
				matrix._C[sourceIndex, nodes.Negative] = -1;
			}

			matrix._E[sourceIndex] = state ? 1 : 0;
		}

		#endregion

		#region Current source activation
		
		/// <summary>
		/// Activates (all current sources are not active by default) the current source given by the index. Assigns 1 to its output in order to
		/// generate admittance matrix that produces transfer function.
		/// </summary>
		/// <param name="sourceIndex"></param>
		private void ActivateCurrentSource(AdmittanceMatrix matrix, ISourceDescription source)
		{
			// Get the nodes
			var nodes = _SourcesNodes[source];
			
			// If the positive terminal is not grounded
			if (nodes.Positive != GroundNodeIndex)
			{
				// Add source's current to the node
				matrix._I[nodes.Positive] = 1;
			}

			// If the negative terminal is not grounded
			if (nodes.Negative != -1)
			{
				// Subtract source's current from the node
				matrix._I[nodes.Negative] = -1;
			}
		}

		#endregion

		#region Node generation

		/// <summary>
		/// Constructs nodes (assigns them to <see cref="_Nodes"/>), finds and removes reference nodes, assigns indices
		/// </summary>
		private void ConstructNodes()
		{
			// Generate nodes using helper class
			_Nodes = NodeGenerator.Generate(_Schematic);

			// Find, assign and remove the reference (ground) nodes
			ProcessReferenceNodes();

			// Assign indices to nodes
			for (int i = 0; i < _Nodes.Count; ++i)
			{
				// Node indexing starts at ground node index + 1
				_Nodes[i].Index = SimulationManager.GroundNodeIndex + 1 + i;
			}
		}

		/// <summary>
		/// Finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected terminals),
		/// removes them from <see cref="_Nodes"/>. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		private void ProcessReferenceNodes()
		{
			// Find all reference nodes
			var referenceNodes = FindReferenceNodes();
						
			// Remove them from the nodes list
			_Nodes.RemoveAll((node) => referenceNodes.Contains(node));

			// Create a node which will be the final reference node
			_ReferenceNode = new Node
			{
				// Assign ground node index to it
				Index = SimulationManager.GroundNodeIndex
			};

			// Merge every node that was determined to be a reference node to the final reference node
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
		private IEnumerable<INode> FindReferenceNodes()
		{
			// Filter the nodes, look for all nodes that have na IGround connected to them
			var referenceNodes = new List<INode>(_Nodes.FindAll((node) => node.ConnectedComponents.Exists((component) => component is IGround)));

			// If any was found, return the list
			if (referenceNodes.Count > 0)
			{
				return referenceNodes;
			}

			// If not, go through all nodes to find a source
			foreach (var node in _Nodes)
			{
				// Find all sources connected to the node
				var sources = new List<ITwoTerminal>(node.ConnectedComponents.
					Where((component) => component is ISource).
					Select((source) => source as ITwoTerminal));

				// Go through each source
				foreach (var source in sources)
				{
					// If its negative (A) terminal is connected to the node, return the node as the reference node
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

		#region Information extraction from schematic

		/// <summary>
		/// Extracts all frequencies from the circuit by checking what's the frequency of every <see cref="IACVoltageSource"/>
		/// </summary>
		private void FindFrequenciesInCircuit()
		{
			// If there are AC voltage sources
			if (_ACVoltageSources.Count > 0)
			{
				// Get all frequencies
				var frequencies = _ACVoltageSources.Select((source) => source.Frequency).Distinct();

				// Take the minimum and maximum and assign them to appropriate properties
				LowestFrequency = frequencies.Min();
				HighestFrequency = frequencies.Max();
			}
			else
			{
				// Otherwise assign 0 to both properties - there are no AC sources
				LowestFrequency = 0;
				HighestFrequency = 0;
			}
		}

		/// <summary>
		/// Finds all active components and assigns their count to <see cref="ActiveComponentsCount"/>
		/// </summary>
		private void InitializeActiveComponents() => ActiveComponentsCount = _Schematic.Components.
			// Find all active components
			Where((component) => component is IActiveComponent).
			Cast<IActiveComponent>().
			Count();

		/// <summary>
		/// Returns all <see cref="IDCVoltageSource"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IDCVoltageSource> FindDCVoltageSources() => _Schematic.Components.
			Where((component) => component is IDCVoltageSource).
			Cast<IDCVoltageSource>();

		/// <summary>
		/// Returns all <see cref="IACVoltageSource"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IACVoltageSource> FindACVoltageSources() => _Schematic.Components.
			Where((component) => component is IACVoltageSource).
			Cast<IACVoltageSource>();

		/// <summary>
		/// Returns all <see cref="ICurrentSource"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ICurrentSource> FindDCCurrentSources() => _Schematic.Components.
			Where((component) => component is ICurrentSource).
			Cast<ICurrentSource>();

		/// <summary>
		/// Returns all <see cref="IOpAmp"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IOpAmp> FindOpAmps() => _Schematic.Components.
			Where((component) => component is IOpAmp).
			Cast<IOpAmp>();

		#endregion

		#region Initialization of private collections

		/// <summary>
		/// Extracts all components that require special care (<see cref="IDCVoltageSource"/>s, <see cref="IACVoltageSource"/>s,
		/// <see cref="ICurrentSource"/>s, <see cref="IOpAmp"/>s) to their respective containers.
		/// </summary>
		private void ExtractSpecialComponents()
		{
			// Get the voltage sources
			_DCVoltageSources = new List<ISourceDescription>(FindDCVoltageSources().Select((x) => x.Description));

			// Get the AC voltage sources
			_ACVoltageSources = new List<ISourceDescription>(FindACVoltageSources().Select((x) => x.Description));

			// Get the current sources
			_DCCurrentSources = new List<ISourceDescription>(FindDCCurrentSources().Select((x) => x.Description));

			// Get the op-amps
			_OpAmps = new List<IOpAmpDescription>(_Schematic.Components.
				Where((component) => component is IOpAmp).
				Cast<IOpAmp>().
				Select((x) => x.Description));

			// Generate source indices collection - DC voltage sources, AC voltage sources and op-amps are generated together - these components are
			// considered to be the same type in admittance matrix. That's why descriptions of these elements are first concatenated (order matters)
			// and then indexed, from 0 till the end of the enumeration of descriptions. Finally DC current sources are added to the collection,
			// those are indexed separately, also starting from 0.
			_IndexedComponentsIndices = _DCVoltageSources.
				Concat(_ACVoltageSources).
				Cast<IComponentDescription>().
				Concat(_OpAmps).
				Select((x, i) => new KeyValuePair<IComponentDescription, int>(x, i)).
				Concat(_DCCurrentSources.Select((x, i) => new KeyValuePair<IComponentDescription, int>(x, i))).
				ToDictionary((x) => x.Key, (x) => x.Value);

			// Prepare the active components
			InitializeActiveComponents();
		}

		/// <summary>
		/// Finds all nodes connected with importnant elements (<see cref="IOpAmp"/>s, <see cref="IDCVoltageSource"/>s,
		/// <see cref="ICurrentSource"/>s) and stores them in dictionaries for an easy and fast look-up
		/// </summary>
		private void FindImportantNodes()
		{
			// Get nodes with reference node (some terminals are grounded - if ground node is not considered List.Find functions wouldn't
			// be successful.
			var nodesWithReference = new List<INode>(GetNodes());

			// Find nodes of all two-terminal sources, to do that make an enumeration of DC voltage sources, AC voltage sources and DC current sources.
			// Create tuples where first item is the source casted to ITwoTerminal (just its terminals are required) and the second item is the
			// source's description
			_SourcesNodes = FindDCVoltageSources().Select((x) => Tuple.Create((ITwoTerminal)x, x.Description)).
				Concat(FindACVoltageSources().Select((x) => Tuple.Create((ITwoTerminal)x, x.Description))).
				Concat(FindDCCurrentSources().Select((x) => Tuple.Create((ITwoTerminal)x, x.Description))).
				// Then make a dictionary out of them
				ToDictionary(
					// Keys are descriptions
					(x) => x.Item2,
					// Values are TwoTerminalSourceNodeInfos with assigned (found) nodes and output value of the source
					(x) => new TwoTerminalSourceNodeInfo(
						nodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.Item1.TerminalB)).Index,
						nodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.Item1.TerminalA)).Index));
			
			// Find nodes of all op-amps and put them in a dictionary
			_OpAmpsNodes = FindOpAmps().ToDictionary(
				// Keys are descriptions
				(x) => x.Description,
				// Values are OpAmpNodeInfos with assigned (found) nodes
				(x) => new OpAmpNodeInfo(
					nodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.TerminalC)).Index,
					nodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.TerminalA)).Index,
					nodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.TerminalB)).Index));
		}

		/// <summary>
		/// Initializes <see cref="_OpAmpOperation"/> with default values - all <see cref="OpAmpOperationMode.Active"/> - all <see cref="IOpAmp"/>s
		/// in active operation
		/// </summary>
		private void InitializeOpAmpOperationCollection() =>
			FindOpAmps().ForEach((x) => _OpAmpOperation.Add(x.Description, OpAmpOperationMode.Active));

		#endregion

		#region Initial OpAmp configuration

		/// <summary>
		/// Modifies <paramref name="matrix"/> with initial op-amp settings - op-amps are set in their respective operation modes depending on
		/// <see cref="_OpAmpOperation"/>.
		/// </summary>
		private void InitialOpAmpConfiguration(AdmittanceMatrix matrix, bool ac)
		{
			FillBMatrixOpAmpOutputNodes(matrix);

			// Configure each op-amp for its operation
			foreach(var opAmp in _OpAmps)
			{
				ConfigureOpAmpOperation(matrix, opAmp, _OpAmpOperation[opAmp], ac);
			}
		}

		#endregion

		#region Correctness checks

		/// <summary>
		/// Checks if all <see cref="IOpAmp"/> have their outputs not grounded. If such thing occurs an exception is thrown
		/// </summary>
		private void CheckOpAmpOutputs()
		{
			foreach (var item in _OpAmpsNodes.Values)
			{
				if (item.Output == GroundNodeIndex)
				{
					throw new Exception("Output of a(n) " + IoC.Resolve<IComponentFactory>().GetDeclaration<IOpAmp>().DisplayName +
						" cannot be grounded");
				}
			}
		}

		#endregion

		#region OpAmps operation adjustment

		/// <summary>
		/// Checks if operation modes of all <see cref="IOpAmp"/>s is correct, if <paramref name="adjust"/> is set to true then adjusts the
		/// <see cref="IOpAmp"/> that did not operate correctly (it does not mean all <see cref="IOpAmp"/>s will operate correctly - iterative
		/// approach is needed).
		/// </summary>
		/// <param name="nodePotentials">Potentials at nodes, keys are simulation node indices</param>
		/// <param name="adjust">True if <see cref="_OpAmpOperation"/> should be adjusted to try and find correct operation modes</param>
		/// <returns></returns>
		private bool CheckOpAmpOperation(IEnumerable<KeyValuePair<int, double>> nodePotentials, bool adjust)
		{
			// Cast the potentials to a dictionary for an easier lookup, add 1 to keys because op-amp nodes are stored with regular indices
			var lookupPotentials = nodePotentials.ToDictionary((x) => x.Key, (x) => x.Value);

			// For each op-amp
			foreach(var opAmp in _OpAmps)
			{
				// And get its output node potential
				var outputVoltage = lookupPotentials[_OpAmpsNodes[opAmp].Output];

				// The operation mode that is expected
				OpAmpOperationMode expectedOperationMode = OpAmpOperationMode.Active;

				// If output voltage is between supply voltages
				if (outputVoltage > opAmp.NegativeSupplyVoltage && outputVoltage < opAmp.PositiveSupplyVoltage)
				{
					// Active operation mode
					expectedOperationMode = OpAmpOperationMode.Active;
				}
				// Else if it's greater than positive supply voltage
				else if (outputVoltage >= opAmp.PositiveSupplyVoltage)
				{
					// Positive saturation
					expectedOperationMode = OpAmpOperationMode.PositiveSaturation;
				}
				else
				{
					// Last possibility - negative saturation
					expectedOperationMode = OpAmpOperationMode.NegativeSaturation;
				}

				// If the expected operation mode differs from the actual on
				if (expectedOperationMode != _OpAmpOperation[opAmp])
				{
					// If adjustment was requested, set the op-amp's operation mode to the expected mode
					if (adjust)
					{
						_OpAmpOperation[opAmp] = expectedOperationMode;
					}

					// Return incorrect operation
					return false;
				}
			}

			// Return correct operation - neither op-amp operated incorrectly
			return true;
		}

		#endregion

		#region Admittance matrix construction

		/// <summary>
		/// Constructs and initializes the general version (without any source in mind) of admittance matrix
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		private AdmittanceMatrix ConstructAndInitialize(double frequency)
		{
			// TODO: Short circuit inductors for DC when they're added
						
			var matrix = new AdmittanceMatrix(_BigDimension, _SmallDimension);

			// Initialize submatrices (arrays)
			InitializeSubmatrices(matrix);

			// Fill A matrix - it's only dependent on frequency
			FillAMatrix(matrix, frequency);

			// Configure voltage sources to be off - the only active voltage source (if any) can then be turned on
			ConfigureVoltageSources(matrix, false);

			// Initialize op-amp settings - active operation
			InitialOpAmpConfiguration(matrix, frequency > 0);

			return matrix;
		}

		/// <summary>
		/// Returns an admittance matrix for voltage source given by <paramref name="source"/>.
		/// The source is regarded as a unity source so that any node potential is in fact the transfer function and can be used to determine
		/// potentials for arbitrary voltage source values (Unode = K(jw) * Usource, for Usource = 1 we have Unode = K(jw) which allows to
		/// obtain K(jw) which can be then substituted to Unode = K(jw) * Usource to obtain Unode for any Usource).
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private AdmittanceMatrix ConstructForVoltageSource(ISourceDescription source)
		{
			// Create initialized matrix
			var matrix = ConstructAndInitialize(source.Frequency);

			// Turn on the voltage source
			ConfigureVoltageSource(matrix, source, true);

			return matrix;
		}

		/// <summary>
		/// Returns an admittance matrix for current source given by <paramref name="source"/>.
		/// The source is regarded as a unity source so that any node potential is in fact the transfer function and can be used to determine
		/// potentials for arbitrary voltage source values (Unode = K(jw) * Usource, for Usource = 1 we have Unode = K(jw) which allows to
		/// obtain K(jw) which can be then substituted to Unode = K(jw) * Usource to obtain Unode for any Usource).
		/// </summary>
		/// <returns></returns>
		private AdmittanceMatrix ConstructForCurrentSource(ISourceDescription source)
		{
			// Create initialized matrix
			var matrix = ConstructAndInitialize(source.Frequency);

			// Turn on the current source
			ActivateCurrentSource(matrix, source);

			return matrix;
		}


		#endregion

		#endregion

		#region Public methods

		/// <summary>
		/// Resets operation of <see cref="IOpAmp"/>s - every <see cref="IOpAmp"/> is set to active mode
		/// </summary>
		public void ResetOpAmpOperation()
		{
			foreach(var opAmp in _OpAmps)
			{
				_OpAmpOperation[opAmp] = OpAmpOperationMode.Active;
			}
		}

		/// <summary>
		/// Checks <see cref="IOpAmp"/>s operation and returns true if it's correct or false if it's incorrect.
		/// </summary>
		/// <param name="nodePotentials">Keys are simulation node indices</param>
		/// <returns></returns>
		public bool CheckOpAmpOperation(IEnumerable<KeyValuePair<int, double>> nodePotentials) => CheckOpAmpOperation(nodePotentials, false);

		/// <summary>
		/// Checks <see cref="IOpAmp"/> operation, returns true if it's correct and false if it's incorrect. Additionally adjusts the
		/// <see cref="IOpAmp"/> that triggered incorrect operation.
		/// </summary>
		/// <param name="nodePotentials">Keys are simulation node indices</param>
		/// <returns></returns>
		public bool CheckOpAmpOperationWithSelfAdjustment(IEnumerable<KeyValuePair<int, double>> nodePotentials) =>
			CheckOpAmpOperation(nodePotentials, true);

		/// <summary>
		/// Constructrs an admittance matrix for the given source - the solution of that admittance matrix are transfer functions for node potentials
		/// and active components currents.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public AdmittanceMatrix Construct(ISourceDescription source)
		{
			switch(source.SourceType)
			{
				case SourceType.ACVoltageSource:
				case SourceType.DCVoltageSource:
					{
						return ConstructForVoltageSource(source);
					}

				case SourceType.DCCurrentSource:
					{
						return ConstructForCurrentSource(source);
					}

				default:
					{
						throw new Exception($"Unhandled {nameof(SourceType)}");
					}
			}
		}

		/// <summary>
		/// Constructs a DC addmittance matrix for saturated <see cref="IOpAmp"/>s only. Its purpose is to determine influence of saturated
		/// <see cref="IOpAmp"/>s on AC circuits.
		/// </summary>
		/// <returns></returns>
		public AdmittanceMatrix ConstructDCForSaturatedOpAmpsOnly1() => ConstructAndInitialize(0);

		/// <summary>
		/// Returns all nodes generated for the <see cref="ISchematic"/> passed to constructor
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INode> GetNodes() => _Nodes.ConcatAtBeginning(_ReferenceNode);

		/// <summary>
		/// Returns all nodes generated for the <see cref="ISchematic"/> passed to constructor without the reference (ground) node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INode> GetNodesWithoutReference() => _Nodes;

		/// <summary>
		/// Returns indices of all nodes generated for the <see cref="ISchematic"/> passed to constructor
		/// </summary>
		/// <returns></returns>
		public IEnumerable<int> GetNodeIndices() => GetNodes().Select((x) => x.Index);

		/// <summary>
		/// Returns indices of all nodes generated for the <see cref="ISchematic"/> passed to constructor without the reference (ground) node.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<int> GetNodeIndicesWithoutReference() => GetNodesWithoutReference().Select((x) => x.Index);
		
		#endregion

		#region Public static properties

		/// <summary>
		/// Index assigned to reference (ground) nodes
		/// </summary>
		public static int GroundNodeIndex { get; } = -1;

		#endregion
	}
}