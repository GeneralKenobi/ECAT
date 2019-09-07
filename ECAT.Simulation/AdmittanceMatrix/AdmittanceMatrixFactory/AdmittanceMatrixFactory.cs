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
	public partial class AdmittanceMatrixFactory
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
		/// <see cref="_Nodes"/> with <see cref="_ReferenceNode"/>
		/// </summary>
		private List<INode> _NodesWithReference => _Nodes.ConcatAtBeginning(_ReferenceNode).ToList();

		/// <summary>
		/// All indices assigned to active components
		/// </summary>
		private IList<int> _ActiveComponentsIndices { get; set; }

		/// <summary>
		/// Node that is used as reference (with potential equal to 0)
		/// </summary>
		private INode _ReferenceNode { get; set; }

		/// <summary>
		/// Contains inductors mapped to DC voltage sources that replace them
		/// </summary>
		private IDictionary<IComponentDescription, IInductor> _MappedInductors { get; set; }

		#endregion

		#region Collections of components/corresponding nodes

		/// <summary>
		/// List with DC voltage sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<IDCVoltageSource> _DCVoltageSources { get; set; }

		/// <summary>
		/// Voltage Sources that replace <see cref="IInductor"/>s for DC
		/// </summary>
		private IList<IDCVoltageSource> _InductorVoltageSources { get; } = new List<IDCVoltageSource>();

		/// <summary>
		/// Voltage sources that represent a saturated <see cref="IBjt"/>
		/// </summary>
		private IDictionary<IBjt, BjtSourcesInfo> _BjtVoltageSources { get; } = new Dictionary<IBjt, BjtSourcesInfo>();

		/// <summary>
		/// List with AC voltage sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<IACVoltageSource> _ACVoltageSources { get; set; }

		/// <summary>
		/// List with DC current sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ICurrentSource> _DCCurrentSources { get; set; }

		/// <summary>
		/// List with descriptions of all DC voltage sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ISourceDescription> _DCVoltageSourcesDescriptions => _DCVoltageSources.Select((x) => x.Description).ToList();

		/// <summary>
		/// List with descriptions of all AC voltage sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ISourceDescription> _ACVoltageSourcesDescriptions => _ACVoltageSources.Select((x) => x.Description).ToList();

		/// <summary>
		/// List with descriptions of all DC current sources in the <see cref="_Schematic"/>.
		/// </summary>
		private IList<ISourceDescription> _DCCurrentSourcesDescriptions => _DCCurrentSources.Select((x) => x.Description).ToList();

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

		/// <summary>
		/// Dictionary containing operation mode assigned to each op-amp.
		/// </summary>
		private IDictionary<ITransistor, TransistorOperationMode> _TransistorOperation { get; } = new Dictionary<ITransistor, TransistorOperationMode>();

		/// <summary>
		/// List containing NPN BJTs
		/// </summary>
		private IList<INpnBjt> _NpnBjts { get; set; }

		/// <summary>
		/// Dictionary with indices of <see cref="IBjt"/>
		/// </summary>
		private IDictionary<IBjt, BjtNodeInfo> _BjtNodes { get; set; }

		/// <summary>
		/// Contains IDs of inner node of each BJT
		/// </summary>
		private IDictionary<IBjt, int> _InnerBjtNodes { get; } = new Dictionary<IBjt, int>();

		#endregion

		#region Admittance matrix dimensions

		/// <summary>
		/// Size of A part of the admittance matrix (equal to number of nodes)
		/// </summary>
		private int _BigDimension => _Nodes.Count;

		/// <summary>
		/// Size of D part of admittance matrix (equal to number of independent voltage sources, with op-amp outputs included)
		/// </summary>
		private int GetSmallDimension(bool includeInductorVoltageSources) =>
			_DCVoltageSources.Count + _ACVoltageSources.Count + _OpAmps.Count + (includeInductorVoltageSources ? _InductorVoltageSources.Count : 0) +
			_BjtVoltageSources.Sum((x) =>
			{
				switch (_TransistorOperation[x.Key])
				{
					case TransistorOperationMode.Cutoff:
						return 1;
					case TransistorOperationMode.Active:
						return 2;
					case TransistorOperationMode.Saturation:
						return 3;
					default:
						return 0;
				}
			});

		#endregion

		#endregion

		#region Public properties

		/// <summary>
		/// Number of nodes (without reference node) created on basis of <see cref="ISchematic"/> passed to constructor.
		/// </summary>
		public int NodesCount => _Nodes.Count;

		/// <summary>
		/// Indices of all generated nodes, without reference node.
		/// </summary>
		public IEnumerable<int> Nodes => _Nodes.Select((x) => x.Index);

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
		public int ACVoltageSourcesCount => _ACVoltageSourcesDescriptions.Count;

		/// <summary>
		/// Number of <see cref="IDCVoltageSource"/>s in the circuit
		/// </summary>
		public int DCVoltageSourcesCount => _DCVoltageSourcesDescriptions.Count;

		/// <summary>
		/// Number of <see cref="ICurrentSource"/>s in the circuit
		/// </summary>
		public int DCCurrentSourcesCount => _DCCurrentSourcesDescriptions.Count;

		/// <summary>
		/// Number of <see cref="IOpAmp"/>s in the circuit
		/// </summary>
		public int OpAmpsCount => _OpAmps.Count;

		/// <summary>
		/// Number of active components in the circuit
		/// </summary>
		public int ActiveComponentsCount => _ActiveComponentsIndices.Count;

		/// <summary>
		/// All indices assigned to active components
		/// </summary>
		public IEnumerable<int> ActiveComponents => _ActiveComponentsIndices;

		/// <summary>
		/// Returns descriptions of AC voltage sources
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISourceDescription> ACVoltageSources => _ACVoltageSourcesDescriptions;

		/// <summary>
		/// Returns descriptions of DC sources
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISourceDescription> DCVoltageSources => _DCVoltageSourcesDescriptions;

		/// <summary>
		/// Returns descriptions of DC sources
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISourceDescription> DCCurrentSources => _DCCurrentSourcesDescriptions;

		/// <summary>
		/// Returns descriptions of all AC sources
		/// </summary>
		public IEnumerable<ISourceDescription> ACSources => _ACVoltageSourcesDescriptions;

		/// <summary>
		/// Sweep source in the schematic
		/// </summary>
		public ISweepVoltageSource SweepSource { get; private set; }

		/// <summary>
		/// Enumeration of voltmeters in the circuit
		/// </summary>
		public IEnumerable<IVoltmeterMeasurement> Voltmeters { get; private set; }

		/// <summary>
		/// <see cref="ISourceDescription"/> for results generated for saturated op-amps (matrices built with
		/// <see cref="ConstructDCForSaturatedComponentsOnly"/>).
		/// </summary>
		public ISourceDescription OpAmpSaturationSource { get; } = new OpAmpSaturationSourceDescription();

		/// <summary>
		/// <see cref="ISourceDescription"/> for results generated for saturated op-amps (matrices built with
		/// <see cref="ConstructDCForSaturatedComponentsOnly"/>).
		/// </summary>
		public ISourceDescription BjtBaseEmitterComponent { get; } = new BjtBaseEmitterComponentDescription();

		/// <summary>
		/// <see cref="ISourceDescription"/>s for BJT Base-emitter sources.
		/// </summary>
		public IEnumerable<ISourceDescription> BjtBaseEmitterSources => _BjtVoltageSources.Select((x) => x.Value.SourceEI.Description);

		/// <summary>
		/// <see cref="IBjts"/>s in the schematic.
		/// </summary>
		public IEnumerable<IBjt> Bjts => _NpnBjts;

		#endregion

		#region Private methods

		/// <summary>
		/// Returns DC voltage sources with or without Inductor voltage sources
		/// </summary>
		/// <param name="includeInductorVoltageSources"></param>
		/// <returns></returns>
		private IEnumerable<IDCVoltageSource> GetDCVoltageSourcesForSimulation(bool includeInductorVoltageSources)
		{
			var result = _DCVoltageSources.ToList();

			foreach(var sources in _BjtVoltageSources)
			{
				result.Add(sources.Value.SourceBI);

				if(_TransistorOperation[sources.Key] != TransistorOperationMode.Cutoff)
				{
					result.Add(sources.Value.SourceEI);
				}

				if(_TransistorOperation[sources.Key] == TransistorOperationMode.Saturation)
				{
					result.Add(sources.Value.SourceCI);
				}
			}

			if(includeInductorVoltageSources)
			{
				result.AddRange(_InductorVoltageSources);
			}

			return result;
		}

		/// <summary>
		/// Returns DC voltage sources with or without Inductor voltage sources
		/// </summary>
		/// <param name="includeInductorVoltageSources"></param>
		/// <returns></returns>
		private IEnumerable<IDCVoltageSource> GetAllDCVoltageSources()
		{
			var result = _DCVoltageSources.ToList();

			foreach (var sources in _BjtVoltageSources)
			{
				result.AddRange(sources.Value.AsEnumerable());
			}

			result.AddRange(_InductorVoltageSources);

			return result;
		}

		/// <summary>
		/// Returns DC voltage sources descriptions with or without Inductor voltage sources
		/// </summary>
		/// <param name="includeInductorVoltageSources"></param>
		/// <returns></returns>
		private IEnumerable<ISourceDescription> GetDCVoltageSourcesDescriptions(bool includeInductorVoltageSources) =>
			GetDCVoltageSourcesForSimulation(includeInductorVoltageSources).Select((x) => x.Description);

		/// <summary>
		/// Returns descriptions of all <see cref="IDCVoltageSource"/>s that model <see cref="IBjt"/>s.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ISourceDescription> GetDescriptionsOfBjtVoltageSources()
		{
			var result = Enumerable.Empty<ISourceDescription>();

			foreach(var bjtSources in _BjtVoltageSources)
			{
				result = result.Concat(bjtSources.Value.AsEnumerable().Select((x) => x.Description));
			}

			return result;
		}
		#region Initialization

		/// <summary>
		/// Builds the factory - it's essential to call this method right after constructor
		/// </summary>
		private void Build()
		{
			ConstructNodes();

			ExtractSpecialComponents();

			InitializeBjtOperationCollection();

			InitializeBjts();

			CreateVoltageSourcesForInductors();

			InitializeIndexedComponents();

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
		private void InitializeSubmatrices(AdmittanceMatrix matrix, bool includeInductorVoltageSources)
		{
			matrix._A = ArrayHelpers.CreateAndInitialize(Complex.Zero, _BigDimension, _BigDimension);

			matrix._B = ArrayHelpers.CreateAndInitialize<Complex>(0, _BigDimension, GetSmallDimension(includeInductorVoltageSources));
			matrix._C = ArrayHelpers.CreateAndInitialize(Complex.Zero, GetSmallDimension(includeInductorVoltageSources), _BigDimension);
			matrix._D = ArrayHelpers.CreateAndInitialize(Complex.Zero, GetSmallDimension(includeInductorVoltageSources), GetSmallDimension(includeInductorVoltageSources));

			matrix._E = ArrayHelpers.CreateAndInitialize(Complex.Zero, GetSmallDimension(includeInductorVoltageSources));
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
			foreach (var node in _Nodes)
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
			foreach (var node1 in _Nodes)
			{
				// Matrix A is symmetrical along the main diagonal so it's only necessary to fill the
				// part below main diagonal and copy the operation to the corresponding entry above the main diagonal
				foreach (var node2 in _Nodes.FindAll((x) => x.Index < node1.Index))
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

		/// <summary>
		/// Fills the data connected with <see cref="ITransistor"/>s
		/// </summary>
		private void ConfigureBjtForSmallSignal(AdmittanceMatrix matrix, IBjt bjt)
		{
			var nodes = _BjtNodes[bjt];

			if(nodes.Base >= 0)
			{
				matrix._A[nodes.Base, nodes.Base] += bjt.Y11;

				if(nodes.Collector >= 0)
				{
					matrix._A[nodes.Base, nodes.Collector] += bjt.Y12;
					matrix._A[nodes.Collector, nodes.Base] += bjt.Y21;
				}
			}

			if(nodes.Collector >= 0)
			{
				matrix._A[nodes.Collector, nodes.Collector] += bjt.Y22;

				if(nodes.Emitter >= 0)
				{
					matrix._A[nodes.Collector, nodes.Emitter] -= bjt.Y21 + bjt.Y22;
					matrix._A[nodes.Emitter, nodes.Collector] -= bjt.Y12 + bjt.Y22;
				}
			}

			if(nodes.Emitter >= 0)
			{
				matrix._A[nodes.Emitter, nodes.Emitter] += bjt.Y11 + bjt.Y12 + bjt.Y21 + bjt.Y22;

				if(nodes.Base >= 0)
				{
					matrix._A[nodes.Base, nodes.Emitter] -= bjt.Y11 + bjt.Y12;
					matrix._A[nodes.Emitter, nodes.Base] -= bjt.Y11 + bjt.Y21;
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
			foreach (var opAmp in _OpAmps)
			{
				// Set the entry in _B corresponding to the output node to 1
				matrix._B[_OpAmpsNodes[opAmp].Output, _IndexedComponentsIndices[opAmp]] = 1;
			}
		}

		#endregion

		#region Op-amp operation mode switching

		/// <summary>
		/// Configures <see cref="IOpAmp"/> for operation in <paramref name="operationMode"/> in <paramref name="matrix"/>. Saturated op-amps remain
		/// inactive after this call - they will appear to be saturated but saturation voltage will be equal to 0. Op-amps have to be manually
		/// activated, for example using <see cref="ActivateSaturatedOpAmps(AdmittanceMatrix)"/>
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="opAmpIndex"></param>
		/// <param name="operationMode"></param>
		private void ConfigureOpAmpOperation(AdmittanceMatrix matrix, IOpAmpDescription opAmp, OpAmpOperationMode operationMode)
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
				case OpAmpOperationMode.NegativeSaturation:
					{
						ConfigureOpAmpForSaturation(matrix, opAmp);
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
			if (nodes.NonInvertingInput != ReferenceNode)
			{
				// Fill the entry in the row corresponding to the op-amp (plus starting row)
				// and column corresponding to the node (positive terminal) with -OpenLoopGain
				matrix._C[index, nodes.NonInvertingInput] = -opAmp.OpenLoopGain;
			}

			// If there exists a node to which TerminalB (inverting input) is connected
			// (it's possible it may not exist due to removed ground node)
			if (nodes.InvertingInput != ReferenceNode)
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
		/// Configures submatrices so that <paramref name="opAmp"/> is considered to work in saturation, however output remains at 0 - op-amps have
		/// to be manually activated, for example using <see cref="ActivateSaturatedOpAmp(AdmittanceMatrix, IOpAmpDescription)"/>.
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="opAmp"></param>
		private void ConfigureOpAmpForSaturation(AdmittanceMatrix matrix, IOpAmpDescription opAmp)
		{
			// Indices of op-amps nodes
			var nodes = _OpAmpsNodes[opAmp];

			// And its index
			var index = _IndexedComponentsIndices[opAmp];

			// If the non-inverting input is not grounded, reset its entry in the _C matrix
			if (nodes.NonInvertingInput != ReferenceNode)
			{
				matrix._C[index, nodes.NonInvertingInput] = 0;
			}

			// If the inverting input is not grounded, reset its entry in the _C matrix
			if (nodes.InvertingInput != ReferenceNode)
			{
				matrix._C[index, nodes.InvertingInput] = 0;
			}

			// And the entry in _C corresponding to the output node to 1
			// It is important that, when non-inverting input is connected directly to the output, the entry in _B
			// corresponding to that node is 1 (and not 0 like the if above would set it). Because this assigning is done after
			// the one for non-inverting input no special conditions are necessary however it's very important to remeber about
			// it if (when) this method is modified
			matrix._C[index, nodes.Output] = 1;
		}

		/// <summary>
		/// Activates all saturated op amps
		/// </summary>
		/// <param name="matrix"></param>
		private void ActivateSaturatedOpAmps(AdmittanceMatrix matrix)
		{
			// For each op-amp
			foreach(var opAmp in _OpAmps)
			{
				// If it's in either saturation
				if (_OpAmpOperation[opAmp] == OpAmpOperationMode.PositiveSaturation || _OpAmpOperation[opAmp] == OpAmpOperationMode.NegativeSaturation)
				{
					// Activate it
					ActivateSaturatedOpAmp(matrix, opAmp);
				}
			}
		}

		/// <summary>
		/// Activates Eollector-Inner voltage source for active and saturated BJTs
		/// </summary>
		/// <param name="matrix"></param>
		private void ActivateBjtsEmitterInnerSource(AdmittanceMatrix matrix)
		{
			foreach (var bjt in _NpnBjts)
			{
				if (_TransistorOperation[bjt] == TransistorOperationMode.Active || _TransistorOperation[bjt] == TransistorOperationMode.Saturation) 
				{
					ConfigureVoltageSource(matrix, _BjtVoltageSources[bjt].SourceEI.Description, true);
				}
			}
		}

		/// <summary>
		/// Activates Collector-Inner voltage source for saturated BJTs
		/// </summary>
		/// <param name="matrix"></param>
		private void ActivateBjtsCollectorInnerSource(AdmittanceMatrix matrix)
		{
			foreach (var bjt in _NpnBjts)
			{
				if (_TransistorOperation[bjt] == TransistorOperationMode.Saturation)
				{
					ConfigureVoltageSource(matrix, _BjtVoltageSources[bjt].SourceCI.Description, true);
				}
			}
		}

		/// <summary>
		/// Activates saturated op-amp - modifies matrix E with saturation voltage in entry corresponding to that op-amp. Does not check if
		/// the op-amp is in fact saturated - assumes that caller checked that.
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="opAmp"></param>
		private void ActivateSaturatedOpAmp(AdmittanceMatrix matrix, IOpAmpDescription opAmp) =>
			// Depending on which supply was exceeded, set the value of the op-amp output to either positive or negative
			// supply voltage, depending on saturaiton (if determined that it is in either saturation).
			// Modify entry in E matrix corresponding to index of the op-amp.
			matrix._E[_IndexedComponentsIndices[opAmp]] = _OpAmpOperation[opAmp] == OpAmpOperationMode.PositiveSaturation ?
				opAmp.PositiveSupplyVoltage : opAmp.NegativeSupplyVoltage;

		#endregion

		#region Voltage source activation

		/// <summary>
		/// Configures all voltage sources for specific operation.
		/// </summary>
		private void ConfigureVoltageSources(AdmittanceMatrix matrix, bool state, bool includeInductorVoltageSources)
		{
			// Take all voltage sources (DC + AC)
			foreach (var source in GetDCVoltageSourcesDescriptions(includeInductorVoltageSources).Concat(_ACVoltageSourcesDescriptions))
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
			if (nodes.Positive != ReferenceNode)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with 1 (positive terminal)
				matrix._B[nodes.Positive, sourceIndex] = 1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with 1 (positive terminal)
				matrix._C[sourceIndex, nodes.Positive] = 1;
			}

			// If the negative terminal is not grounded
			if (nodes.Negative != ReferenceNode)
			{
				// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
				// with -1 (negative terminal)
				matrix._B[nodes.Negative, sourceIndex] = -1;

				// Fill the entry in the row corresponding to the source (plus starting row)
				// and column corresponding to the node with -1 (negative terminal)
				matrix._C[sourceIndex, nodes.Negative] = -1;
			}

			matrix._E[sourceIndex] = state ? source.OutputValue : 0;
		}

		#endregion

		#region Current source activation

		/// <summary>
		/// Activates (all current sources are not active by default) the current source given by the index.
		/// </summary>
		/// <param name="sourceIndex"></param>
		private void ActivateCurrentSource(AdmittanceMatrix matrix, ISourceDescription source)
		{
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//                                                                                                                                         //
			// Matrix I has += and -= instead of just assignment because that's the actual correct way of building the matrix.                         //
			// However it only matters if there is more than one source active at a time. For example, if there would be 2 sources, each connected     //
			// to the same node, then the value in the corresponding entry in I matrix should be a sum of produced currents. Simply assigning value    //
			// would result in an error. Again, for now, admittance matrices are built only for one source, however if it would happen that it changes //
			// in the future then there won't be any errors because of assigning rather than adding / subtracting.                                     //
			//                                                                                                                                         //
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			// Get the nodes
			var nodes = _SourcesNodes[source];

			// If the positive terminal is not grounded
			if (nodes.Positive != ReferenceNode)
			{
				// Add source's current to the node
				matrix._I[nodes.Positive] += source.OutputValue;
			}

			// If the negative terminal is not grounded
			if (nodes.Negative != -1)
			{
				// Subtract source's current from the node
				matrix._I[nodes.Negative] -= source.OutputValue;
			}
		}

		#endregion

		#region Node generation

		/// <summary>
		/// Generates inner nodes (nodes only used for simulation) for BJTs
		/// </summary>
		private void CreateInnerNodesForBjts(IEnumerable<IBjt> bjts)
		{
			foreach(var bjt in bjts)
			{
				var node = new Node();
				node.Index = _Nodes.Count;
				node.ConnectedComponents.Add(bjt);
				_InnerBjtNodes.Add(bjt, node.Index);
				_Nodes.Add(node);
			}
		}

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
				_Nodes[i].Index = ReferenceNode + 1 + i;
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
				Index = ReferenceNode
			};

			// Merge every node that was determined to be a reference node to the final reference node
			referenceNodes.ForEach((node) => _ReferenceNode.Merge(node));

			_ReferenceNode.ConnectedTerminals.ForEach((x) => x.NodeIndex = -1);
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
			if (_ACVoltageSourcesDescriptions.Count > 0)
			{
				// Get all frequencies
				var frequencies = _ACVoltageSourcesDescriptions.Select((source) => source.Frequency).Distinct();

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
		/// Finds all active components and assigns their count to <see cref="ActiveComponentsCount"/>. Collections of component descriptions have
		/// to be initialized before call to this method.
		/// </summary>
		private void InitializeIndexedComponents()
		{
			// Generate indices collection for active components - DC voltage sources, AC voltage sources and op-amps are generated together - these
			// components are considered to be the same type in admittance matrix. That's why descriptions of these elements are first concatenated
			// (order matters) and then indexed, from 0 till the end of the enumeration of descriptions.
			var activeComponents = _DCVoltageSourcesDescriptions.
				Concat(_ACVoltageSourcesDescriptions).
				Cast<IComponentDescription>().
				Concat(_OpAmps).
				Concat(GetDescriptionsOfBjtVoltageSources()).
				// Inductor voltage sources have to be last because they're not considered for AC simulations
				Concat(_InductorVoltageSources.Select((x) => x.Description)).
				Select((x, i) => new KeyValuePair<IComponentDescription, int>(x, i));

			// Assign the indices to public property
			_ActiveComponentsIndices = activeComponents.Select((x) => x.Value).ToList();

			// Create a final collection - base in on previously determined indexing for active components and add
			// DC current sources, those are indexed separately, also starting from 0.
			_IndexedComponentsIndices = activeComponents.
				Concat(_DCCurrentSourcesDescriptions.Select((x, i) => new KeyValuePair<IComponentDescription, int>(x, i))).
				ToDictionary((x) => x.Key, (x) => x.Value);

			// Propagate active component indices from 'fake' DC voltage sources to the inductors they simulate
			if(_MappedInductors != null && _MappedInductors.Count > 0)
			{
				foreach(var component in _IndexedComponentsIndices)
				{
					if(_MappedInductors.ContainsKey(component.Key))
					{
						_MappedInductors[component.Key].Index = component.Value;
					}
				}
			}

			foreach(var component in FindDCVoltageSources().Select((x) => Tuple.Create((IActiveComponent)x, (IComponentDescription)x.Description)).
				Concat(FindACVoltageSources().Select((x) => Tuple.Create((IActiveComponent)x, (IComponentDescription)x.Description))).
				Concat(FindOpAmps().Select((x) => Tuple.Create((IActiveComponent)x, (IComponentDescription)x.Description))))
			{
				component.Item1.Index = _IndexedComponentsIndices[component.Item2];
			}
		}

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

		private ISweepVoltageSource FindSweepVoltageSource() => _Schematic.Components.
			Where((component) => component is ISweepVoltageSource).
			Cast<ISweepVoltageSource>().
			FirstOrDefault();

		/// <summary>
		/// Returns all <see cref="IOpAmp"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IOpAmp> FindOpAmps() => _Schematic.Components.
			Where((component) => component is IOpAmp).
			Cast<IOpAmp>();

		/// <summary>
		/// Returns all <see cref="IOpAmp"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<T> FindComponents<T>() => _Schematic.Components.
			Where((component) => component is T).
			Cast<T>();

		/// <summary>
		/// Returns all <see cref="IVoltmeter"/>s present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IVoltmeter> FindVoltmeters() => _Schematic.Components.
			Where((component) => component is IVoltmeter).
			Cast<IVoltmeter>();

		/// <summary>
		/// Returns all <see cref="IInductor"/> present in <see cref="ISchematic"/>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IInductor> FindInductors() => _Schematic.Components.
			Where((component) => component is IInductor).
			Cast<IInductor>();

		/// <summary>
		/// Replaces <see cref="IInductor"/>s with <see cref="IDCVoltageSource"/>s
		/// (for DC simulation purposes Inductors act as short circuit, they are replaced with DC voltage sources
		/// in order to calculate the current flowing through them).
		/// </summary>
		private void CreateVoltageSourcesForInductors()
		{
			_MappedInductors = new Dictionary<IComponentDescription, IInductor>();

			foreach (var inductor in FindInductors())
			{
				var nodeA = _NodesWithReference.Find((x) => x.Index == inductor.TerminalA.NodeIndex);
				var nodeB = _NodesWithReference.Find((x) => x.Index == inductor.TerminalB.NodeIndex);
				var source = CreateVoltageSource(nodeA, nodeB, 0, GetSmallDimension(true));

				_MappedInductors.Add(source.Description, inductor);
				_InductorVoltageSources.Add(source);
			}
		}

		/// <summary>
		/// Replaces <see cref="ITransistor"/>s with <see cref="IDCVoltageSource"/>s
		/// which sets the <see cref="ITransistor"/> into saturation mode.
		/// </summary>
		private void InitializeBjts()
		{
			CreateInnerNodesForBjts(_NpnBjts.Where((x) => !x.SmallSignalModel));

			_BjtNodes = FindComponents<INpnBjt>().ToDictionary((x) => (IBjt)x, (x) => new BjtNodeInfo(
				_NodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.TerminalA)).Index,
				_NodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.TerminalB)).Index,
				_NodesWithReference.Find((node) => node.ConnectedTerminals.Contains(x.TerminalC)).Index,
				_InnerBjtNodes[x]));

			_NpnBjts.ForEach((bjt, i) =>
				{
					if (_TransistorOperation[bjt] != TransistorOperationMode.SmallSignal)
					{
						InitializeBjt(bjt, GetSmallDimension(true) + i);
					}
				});
		}

		/// <summary>
		/// Replaces <see cref="ITransistor"/>s with <see cref="IDCVoltageSource"/>s
		/// which sets the <see cref="ITransistor"/> into saturation mode.
		/// </summary>
		private void PreConfigureBjts(AdmittanceMatrix matrix)
		{
			foreach(var bjt in _NpnBjts)
			{
				if(_TransistorOperation[bjt] != TransistorOperationMode.SmallSignal)
				{
					ConfigureVoltageSource(matrix, _BjtVoltageSources[bjt].SourceBI.Description, false);

					if(_TransistorOperation[bjt] != TransistorOperationMode.Cutoff)
					{
						ConfigureVoltageSource(matrix, _BjtVoltageSources[bjt].SourceEI.Description, false);
					}

					if(_TransistorOperation[bjt] == TransistorOperationMode.Saturation)
					{
						ConfigureVoltageSource(matrix, _BjtVoltageSources[bjt].SourceCI.Description, false);
					}
				}
			}
		}

		/// <summary>
		/// Replaces <see cref="ITransistor"/>s with <see cref="IDCVoltageSource"/>s
		/// which sets the <see cref="ITransistor"/> into saturation mode.
		/// </summary>
		private void ConfigureBjts(AdmittanceMatrix matrix)
		{
			foreach (var bjt in _NpnBjts)
			{				
				switch(_TransistorOperation[bjt])
				{
					case TransistorOperationMode.Active:
						{
							ConfigureBjtForActive(matrix, bjt);
						}
						break;

					case TransistorOperationMode.SmallSignal:
						{
							ConfigureBjtForSmallSignal(matrix, bjt);
						}
						break;
				}
			}
		}

		/// <summary>
		/// Alters B sub-matrix to make the bjt operate in active mode
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="bjt"></param>
		private void ConfigureBjtForActive(AdmittanceMatrix matrix, IBjt bjt)
		{
			var nodes = _BjtNodes[bjt];
			var sourceBIIndex = _BjtVoltageSources[bjt].SourceBI.Index;

			matrix._B[nodes.Collector, sourceBIIndex] += bjt.Beta;
			matrix._B[nodes.Inner, sourceBIIndex] -= bjt.Beta;
		}

		/// <summary>
		/// Replaces <see cref="ITransistor"/>s with <see cref="IDCVoltageSource"/>s
		/// which sets the <see cref="ITransistor"/> into saturation mode.
		/// </summary>
		private void InitializeBjt(IBjt bjt, int sourceIndex)
		{
			var nodes = _BjtNodes[bjt];

			var baseNode = _NodesWithReference.Find((x) => x.Index == nodes.Base);
			var collectorNode = _NodesWithReference.Find((x) => x.Index == nodes.Collector);
			var emitterNode = _NodesWithReference.ToList().Find((x) => x.Index == nodes.Emitter);
			var innerNode = _NodesWithReference.ToList().Find((x) => x.Index == nodes.Inner);

			var sourceBI = CreateVoltageSource(innerNode, baseNode, 0, sourceIndex);
			var sourceEI = CreateVoltageSource(emitterNode, innerNode, bjt.UBEForward, sourceIndex + 1);
			var sourceCI = CreateVoltageSource(collectorNode, innerNode, bjt.UBEForward - bjt.UCESaturation, sourceIndex + 2);

			_BjtVoltageSources.Add(bjt, new BjtSourcesInfo(sourceBI, sourceEI, sourceCI));
		}

		/// <summary>
		/// Creates a <see cref="IDCVoltageSource"/> between thxe provided nodes.
		/// </summary>
		/// <param name="referenceNode"></param>
		/// <param name="outputNode"></param>
		/// <param name="outputVoltage"></param>
		/// <returns></returns>
		private IDCVoltageSource CreateVoltageSource(INode referenceNode, INode outputNode, double outputVoltage, int index)
		{
			var source = IoC.Resolve<IComponentFactory>().Construct<IDCVoltageSource>() as IDCVoltageSource;

			source.TerminalB.NodeIndex = outputNode.Index;
			source.TerminalA.NodeIndex = referenceNode.Index;
			outputNode.ConnectedTerminals.Add(source.TerminalB);
			referenceNode.ConnectedTerminals.Add(source.TerminalA);
			outputNode.ConnectedComponents.Add(source);
			referenceNode.ConnectedComponents.Add(source);
			source.OutputValue = outputVoltage;
			source.Index = index;

			return source;
		}

		#endregion

		#region Initialization of private collections

		/// <summary>
		/// Extracts all components that require special care (<see cref="IDCVoltageSource"/>s, <see cref="IACVoltageSource"/>s,
		/// <see cref="ICurrentSource"/>s, <see cref="IOpAmp"/>s) to their respective containers.
		/// </summary>
		private void ExtractSpecialComponents()
		{
			// Get the voltage sources
			_DCVoltageSources = new List<IDCVoltageSource>(FindDCVoltageSources());

			// Get the AC voltage sources
			_ACVoltageSources = new List<IACVoltageSource>(FindACVoltageSources());

			// Get the current sources
			_DCCurrentSources = new List<ICurrentSource>(FindDCCurrentSources());

			SweepSource = FindSweepVoltageSource();

			// Get the op-amps
			_OpAmps = _Schematic.Components.
				Where((component) => component is IOpAmp).
				Cast<IOpAmp>().
				Select((x) => x.Description).
				ToList();

			_NpnBjts = FindComponents<INpnBjt>().ToList();

			// Get voltmeters
			Voltmeters = FindVoltmeters().Select((x) => IoC.Resolve<IVoltmeterMeasurement>(x.ID, x.TerminalA.NodeIndex, x.TerminalB.NodeIndex));
		}

		/// <summary>
		/// Finds all nodes connected with importnant elements (<see cref="IOpAmp"/>s, <see cref="IDCVoltageSource"/>s,
		/// <see cref="ICurrentSource"/>s) and stores them in dictionaries for an easy and fast look-up
		/// </summary>
		private void FindImportantNodes()
		{
			// Get nodes with reference node (some terminals are grounded - if ground node is not considered List.Find functions wouldn't
			// be successful.
			var nodesWithReference = _Nodes.Concat(_ReferenceNode).ToList();

			// Find nodes of all two-terminal sources, to do that make an enumeration of DC voltage sources, AC voltage sources and DC current sources.
			// Create tuples where first item is the source casted to ITwoTerminal (just its terminals are required) and the second item is the
			// source's description
			var temp = GetDCVoltageSourcesForSimulation(true).Select((x) => Tuple.Create((ITwoTerminal)x, x.Description)).
				Concat(_ACVoltageSources.Select((x) => Tuple.Create((ITwoTerminal)x, x.Description))).
				Concat(_DCCurrentSources.Select((x) => Tuple.Create((ITwoTerminal)x, x.Description)));
			_SourcesNodes = GetAllDCVoltageSources().Select((x) => Tuple.Create((ITwoTerminal)x, x.Description)).
				Concat(_ACVoltageSources.Select((x) => Tuple.Create((ITwoTerminal)x, x.Description))).
				Concat(_DCCurrentSources.Select((x) => Tuple.Create((ITwoTerminal)x, x.Description))).
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

		/// <summary>
		/// Initializes <see cref="_TransistorOperation"/> with default values - all <see cref="TransistorOperationMode.Active"/> - all <see cref="ITransistor"/>s
		/// in active operation
		/// </summary>
		private void InitializeBjtOperationCollection() =>
			FindComponents<ITransistor>().ForEach((x) => _TransistorOperation.Add(
				x, x.SmallSignalModel ? TransistorOperationMode.SmallSignal : TransistorOperationMode.Active));

		#endregion

		#region Initial OpAmp configuration

		/// <summary>
		/// Modifies <paramref name="matrix"/> with initial op-amp settings - op-amps are set in their respective operation modes depending on
		/// <see cref="_OpAmpOperation"/>.
		/// </summary>
		private void InitialOpAmpConfiguration(AdmittanceMatrix matrix)
		{
			FillBMatrixOpAmpOutputNodes(matrix);

			// Configure each op-amp for its operation
			foreach(var opAmp in _OpAmps)
			{
				ConfigureOpAmpOperation(matrix, opAmp, _OpAmpOperation[opAmp]);
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
				if (item.Output == ReferenceNode)
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
		private bool CheckOperation(IEnumerable<KeyValuePair<int, double>> nodePotentials, IEnumerable<KeyValuePair<int, double>> sourcesCurrents, bool adjust)
		{
			// Cast the potentials to a dictionary for an easier lookup, add 1 to keys because op-amp nodes are stored with regular indices
			var lookupPotentials = nodePotentials.ToDictionary((x) => x.Key, (x) => x.Value);
			var lookupCurrents = sourcesCurrents.ToDictionary((x) => x.Key, (x) => x.Value);

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

			foreach (var bjt in _NpnBjts)
			{
				double basePotential = _BjtNodes[bjt].Base >= 0 ? lookupPotentials[_BjtNodes[bjt].Base] : 0;
				double collectorPotential = _BjtNodes[bjt].Collector >= 0 ? lookupPotentials[_BjtNodes[bjt].Collector] : 0;
				double emitterPotential = _BjtNodes[bjt].Emitter >= 0 ? lookupPotentials[_BjtNodes[bjt].Emitter] : 0;
				// // Minus becaue current source's current is given in the opposite direction of voltage drop
				double baseCurrent = lookupCurrents[_BjtVoltageSources[bjt].SourceBI.Index];

				var uBE = basePotential - emitterPotential;
				var uCE = collectorPotential - emitterPotential;

				TransistorOperationMode expectedOperationMode = TransistorOperationMode.Active;

				// If output voltage is between supply voltages
				if(baseCurrent <= 0)
				{
					expectedOperationMode = TransistorOperationMode.Cutoff;
				}
				else if (uCE <= bjt.UCESaturation)
				{
					expectedOperationMode = TransistorOperationMode.Saturation;
				}
				// If not cutoff or saturation then the transistor must be in active mode - already assigned
				
				// If the expected operation mode differs from the actual on
				if (expectedOperationMode != _TransistorOperation[bjt])
				{
					// If adjustment was requested, set the op-amp's operation mode to the expected mode
					if (adjust)
					{
						_TransistorOperation[bjt] = expectedOperationMode;
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
			bool includeInductorVoltageSources = frequency == 0;
						
			var matrix = new AdmittanceMatrix(_BigDimension, GetSmallDimension(includeInductorVoltageSources));

			// Initialize submatrices (arrays)
			InitializeSubmatrices(matrix, includeInductorVoltageSources);

			PreConfigureBjts(matrix);

			// Fill A matrix - it's only dependent on frequency
			FillAMatrix(matrix, frequency);

			// Configure voltage sources to be off - the only active voltage source (if any) can then be turned on
			ConfigureVoltageSources(matrix, false, includeInductorVoltageSources);

			// Initialize op-amp settings - disabled saturated op-amps
			InitialOpAmpConfiguration(matrix);

			ConfigureBjts(matrix);

			return matrix;
		}

		/// <summary>
		/// Returns an admittance matrix for voltage source given by <paramref name="source"/>.
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
		/// Returns descriptions of all DC sources
		/// </summary>
		public IEnumerable<ISourceDescription> GetDCSources() => _DCVoltageSources.Concat(_InductorVoltageSources).Select((x) => x.Description);

		/// <summary>
		/// Descriptions of all sources
		/// </summary>
		public IEnumerable<ISourceDescription> GetAllSources() => _ACVoltageSourcesDescriptions.Concat(GetDCSources());

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
		/// Resets operation of <see cref="IBjt"/>s - every <see cref="IBjt"/> is set to active mode
		/// </summary>
		public void ResetBjtOperation()
		{
			foreach (var bjt in _NpnBjts)
			{
				_TransistorOperation[bjt] = TransistorOperationMode.Active;
			}
		}

		/// <summary>
		/// Checks <see cref="IOpAmp"/>s operation and returns true if it's correct or false if it's incorrect.
		/// </summary>
		/// <param name="nodePotentials">Keys are simulation node indices</param>
		/// <returns></returns>
		public bool CheckOpAmpOperation(IEnumerable<KeyValuePair<int, double>> nodePotentials, IEnumerable<KeyValuePair<int, double>> sourcesCurrents) =>
			CheckOperation(nodePotentials, sourcesCurrents, false);

		/// <summary>
		/// Checks <see cref="IOpAmp"/> operation, returns true if it's correct and false if it's incorrect. Additionally adjusts the
		/// <see cref="IOpAmp"/> that triggered incorrect operation.
		/// </summary>
		/// <param name="nodePotentials">Keys are simulation node indices</param>
		/// <returns></returns>
		public bool CheckOperationWithSelfAdjustment(IEnumerable<KeyValuePair<int, double>> nodePotentials, IEnumerable<KeyValuePair<int, double>> sourcesCurrents) =>
			CheckOperation(nodePotentials, sourcesCurrents, true);

		/// <summary>
		/// Constructrs an admittance matrix for the given source.
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
		/// Constructs an Admittance Matrix for all <paramref name="bjt"/>s' base-emitter DC component.
		/// </summary>
		/// <param name="bjt"></param>
		/// <returns></returns>
		public AdmittanceMatrix ConstructForBjtBESources()
		{
			var matrix = ConstructAndInitialize(0);

			_NpnBjts.Where((bjt) => _TransistorOperation[bjt] == TransistorOperationMode.Active || _TransistorOperation[bjt] == TransistorOperationMode.Saturation).
				ForEach((bjt) => ConfigureVoltageSource(matrix, _BjtVoltageSources[bjt].SourceEI.Description, true));

			return matrix;
		}

		/// <summary>
		/// Constructs a DC addmittance matrix for saturated <see cref="IOpAmp"/>s only. Its purpose is to determine influence of saturated
		/// <see cref="IOpAmp"/>s on AC circuits.
		/// </summary>
		/// <returns></returns>
		public AdmittanceMatrix ConstructDCForSaturatedComponentsOnly()
		{
			// Construct initial version
			var matrix = ConstructAndInitialize(0);

			// And activate saturated op-amps
			ActivateSaturatedOpAmps(matrix);
			ActivateBjtsCollectorInnerSource(matrix);

			return matrix;
		}

		public AdmittanceMatrix ConstructDCForBjtBaseEmitterSources()
		{
			// Construct initial version
			var matrix = ConstructAndInitialize(0);

			// And activate saturated op-amps
			ActivateBjtsEmitterInnerSource(matrix);

			return matrix;
		}

		#endregion

		#region Public static properties

		/// <summary>
		/// Index assigned to reference (ground) nodes
		/// </summary>
		public static int ReferenceNode { get; } = -1;

		#endregion
	}
}