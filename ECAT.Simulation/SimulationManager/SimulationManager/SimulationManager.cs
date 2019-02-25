using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of the simulation module
	/// </summary>
	[RegisterAsInstance(typeof(ISimulationManager))]
	public class SimulationManager : ISimulationManager
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired whenever simulation completes
		/// </summary>
		public event EventHandler<SimulationCompletedEventArgs> SimulationCompleted;

		#endregion

		#region Private methods

		#region DC biasing
		
		/// <summary>
		/// Topmost logic behind DC bias - calling it will result in simulation being performed and saved to <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		private void DCBiasLogic(AdmittanceMatrixFactory factory)
		{
			// Create dictionary holding final values of potentials at nodes
			var totalPotentials = new Dictionary<INode, IPhasorDomainSignal>();

			// Create dictionary holding final values of currents
			var totalActiveComponentsCurrents = new Dictionary<int, IPhasorDomainSignal>();

			bool opAmpOperationCorrect = true;

			InstantenousState combined = null;

			// Loop that will find correct op-amp settings - initial settings are tested and, if not correct, adjusted. This process
			// goes on until the correct configuration is found.
			do
			{
				// Get the DC transfer functions, combine potentials from all sources to check op-amp operaiton
				combined = GetPhasorsForAllDCSources(factory).ToDC().Combine();

				// Check if op-amps operate correctly with self-adjustment
				opAmpOperationCorrect = factory.CheckOpAmpOperationWithSelfAdjustment(combined.Potentials);
			} while (!opAmpOperationCorrect);

			// Create signal for reference node
			totalPotentials.Add(factory.GetNodes().First(), IoC.Resolve<IPhasorDomainSignal>(0d));
			
			// For each calculated node potential
			foreach(var node in factory.GetNodesWithoutReference())
			{
				totalPotentials.Add(node, IoC.Resolve<IPhasorDomainSignal>(combined.Potentials[node.Index]));
			}

			// For each active component
			foreach(var current in combined.Currents)
			{
				// Create a phasor domain signal for its current
				totalActiveComponentsCurrents.Add(current.Key, IoC.Resolve<IPhasorDomainSignal>(current.Value));
			}

			// Create simulation results based on determined node potentials and active components currents
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsBias(totalPotentials, totalActiveComponentsCurrents);
		}

		#endregion

		/// <summary>
		/// Creates and solves DC admittance matrix for saturated op-amps
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		private InstantenousState GetOpAmpSaturationBias(AdmittanceMatrixFactory factory)
		{
			var matrix = factory.ConstructDCForSaturatedOpAmpsOnly();

			matrix.Solve(out var nodePotentials, out var activeComponentsCurrents);

			var result = new InstantenousState(factory.GetNodeIndicesWithoutReference(), factory.ActiveComponentsCount, a.Singleton);

			result.AddValues(nodePotentials.Select((x) => x.Real).ToArray(), activeComponentsCurrents.Select((x) => x.Real).ToArray());

			return result;
		}

		#region Transfer functions

		/// <summary>
		/// Returns phasors constructed for source given by <paramref name="sourceIndex"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="sourceIndex"></param>
		/// <param name="nodePotentialTransferFunctions"></param>
		/// <param name="activeComponentsCurrentsTransferFunctions"></param>
		private PhasorState GetPhasor(AdmittanceMatrixFactory factory, ISourceDescription sourceDescription)
		{
			var state = new PhasorState(factory.NodesCount, factory.ActiveComponentsCount, sourceDescription);

			// Create an admittance matrix corresponding to the given source
			factory.Construct(sourceDescription).
				Solve(out var nodePotentialTransferFunctions, out var activeComponentsCurrentsTransferFunctions);

			state.AddValues(nodePotentialTransferFunctions, activeComponentsCurrentsTransferFunctions);

			return state;
		}

		/// <summary>
		/// Helper for functions that return phasors generated for many sources. Returns a <see cref="PhasorPartialStates"/> that contains
		/// phasors for all sources in <paramref name="sources"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="sources">Enumeration of sources to construct transfer functions for. All sources listed have to be present in
		/// <paramref name="factory"/>, otherwise an exception will be thrown.</param>
		/// <returns></returns>
		private PhasorPartialStates GetAllPhasorsHelper(AdmittanceMatrixFactory factory, IEnumerable<ISourceDescription> sources)
		{
			var result = new PhasorPartialStates(factory.NodesCount, factory.ActiveComponentsCount, sources);

			// For each AC source
			foreach (var source in sources)
			{
				// Get transfer function
				result.States[source] = GetPhasor(factory, source);
			}

			return result;
		}

		/// <summary>
		/// Returns phasors with transfer functions for all AC sources in <paramref name="factory"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="nodePotentialTransferFunctions"></param>
		/// <param name="activeComponentsCurrentsTransferFunctions"></param>
		private PhasorPartialStates GetPhasorsForAllACSources(AdmittanceMatrixFactory factory) =>
			GetAllPhasorsHelper(factory, factory.ACVoltageSources);

		/// <summary>
		/// Returns phasors with transfer functions for all DC sources in <paramref name="factory"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="nodePotentialTransferFunctions"></param>
		/// <param name="activeComponentsCurrentsTransferFunctions"></param>
		private PhasorPartialStates GetPhasorsForAllDCSources(AdmittanceMatrixFactory factory) =>
			GetAllPhasorsHelper(factory, factory.DCSources);
		
		#endregion
		
		#region AC full cycle helpers
		
		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="factory"/>.
		/// Returns calculated node potentials and active component currents as sinusoidal waveforms.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="totalPotentials">Calculated instantenous values of potentials at nodes</param>
		/// <param name="totalActiveComponentsCurrents">Calculated instantenous values of active components' currents</param>
		/// <param name="pointsCount">Number of points to calculate for the signals</param>
		/// <param name="timeStep">Time step between two calculational points</param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results, if false only saturated <see cref="IOpAmp"/>s
		/// will be considered for DC part of simulation</param>
		private WaveformPartialState FullCycleHelper(AdmittanceMatrixFactory factory, int pointsCount, double timeStep, bool includeDCBias = false)
		{
			// Get nodes constructed on the basis of the circuit
			var nodes = factory.GetNodes();

			// Get nodes without the reference node and group them into a list for easier access with indexes
			var nodesWithoutReference = factory.GetNodesWithoutReference().ToList();

			// Get AC transfer functions
			var systemState = GetPhasorsForAllACSources(factory);

			if (includeDCBias)
			{
				// As well as DC transfer functions, if requested
				systemState.MergeWith(GetPhasorsForAllDCSources(factory));
			}

			return systemState.ToWaveform(pointsCount, timeStep);
		}

		private class a : ISourceDescription
		{
			public double Frequency => 0;

			public SourceType SourceType => SourceType.DCVoltageSource;

			public double OutputValue => 0;

			public IIDLabel Label => null;

			public static a Singleton { get; } = new a();
		}

		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="factory"/>.
		/// After determining the transfer functions calculates only one set of instantenous values for point described by
		/// <paramref name="pointIndex"/> and <paramref name="timeStep"/>. Returns an array, each entry corresponds to one
		/// <see cref="IACVoltageSource"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="pointIndex">Index of the point for which to calculate instantenous values</param>
		/// <param name="timeStep">Time step between two calculational points</param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results, if false only saturated <see cref="IOpAmp"/>s
		/// will be considered for DC part of simulation</param>
		private InstantenousPartialStates FullCycleInstantenousValuesHelper(AdmittanceMatrixFactory factory, int pointIndex, double timeStep,
			bool includeDCBias = false)
		{
			// Get indices of nodes, without the reference node, and group them into a list for easier access
			var nodeIndices = factory.GetNodeIndicesWithoutReference().ToList();

			// Result container that will be returned
			// TODO: Provide DC voltage sources descriptions
			var result = new InstantenousPartialStates(nodeIndices, factory.ActiveComponentsCount, factory.AllSources);

			var phasors = GetPhasorsForAllACSources(factory);

			if(includeDCBias)
			{
				phasors.MergeWith(GetPhasorsForAllDCSources(factory));
			}

			var inst = phasors.ToInstantenousValue(pointIndex, timeStep);

			var opAmpBias = GetOpAmpSaturationBias(factory);

			//factory.ResetOpAmpOperation();

			var phasorsBeforeAdjustment = GetPhasorsForAllACSources(factory);

			inst.States.Add(a.Singleton, opAmpBias);

			foreach (var index in factory.GetNodeIndicesWithoutReference())
			{
				var sum = inst.States.Values.Sum((x) => x.Potentials[index]);

				inst.States.Values.ForEach((x) => x.Potentials[index] += opAmpBias.Potentials[index]);
			}

			foreach (var index in factory.ActiveComponentsIndices)
			{
				var sum = inst.States.Values.Sum((x) => x.Currents[index]);

				inst.States.Values.ForEach((x) => x.Currents[index] += opAmpBias.Currents[index]);
			}

			return inst;
		}

		#endregion

		#region Admittance matrix factory simulation settings resolver

		/// <summary>
		/// Calculates time step for a simulation with given number of points and lowest frequency in the circuit so that full simulation
		/// produces exactly one full period of the lowest frequency signal.
		/// </summary>
		/// <param name="pointsCount">Number of points in the simulation</param>
		/// <param name="lowestFrequency">Lowest frequency in simulation</param>
		/// <returns></returns>
		private double GetTimeStep(int pointsCount, double lowestFrequency) =>
			// Time step between two subsequent points in time vector
			1 / (lowestFrequency * pointsCount);

		#endregion

		#region AC (DC) full cycle logics without op-amp adjustment

		/// <summary>
		/// Topmost logic behind AC Full Cycle - calling it will result in simulation being performed and saved to
		/// <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results, if false only saturated <see cref="IOpAmp"/>s
		/// will be considered for DC part of simulation</param>
		private void FullCycleWithoutOpAmpAdjustment(AdmittanceMatrixFactory factory, bool includeDCBias)
		{
			// TODO: number of points should be given by caller
			int pointsCount = 600;

			// Get highest/lowest frequencies in the circuit
			var lowestFrequency = factory.LowestFrequency;

			// Time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, lowestFrequency);

			// Use helper to get the state of the system
			var state = FullCycleHelper(factory, pointsCount, timeStep, includeDCBias);

			var nodes = factory.GetNodes().ToList();

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				state.PotentialsToTimeDomainSignals(timeStep).
				Concat(new KeyValuePair<int, ITimeDomainSignal>(GroundNodeIndex, IoC.Resolve<ITimeDomainSignal>(pointsCount, timeStep))).
				ToDictionary((x) => nodes.Find((node) => node.Index == x.Key), (x) => x.Value),				
				state.CurrentsToTimeDomainSignals(timeStep),
				timeStep,
				0);
		}

		#endregion

		#region AC (DC) full cycle logics with op-amp adjustment

		/// <summary>
		/// Topmost logic behind AC Full Cycle - calling it will result in simulation being performed and saved to
		/// <see cref="ISimulationResultsProvider"/>. This version adjusts <see cref="IOpAmp"/> operation for every calculated point so that neither
		/// <see cref="IOpAmp"/> exceeds its supply voltages.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results, if false only saturated <see cref="IOpAmp"/>s
		/// will be considered for DC part of simulation</param>
		private void FullCycleLogicWithOpAmpAdjustment(AdmittanceMatrixFactory factory, bool includeDCBias)
		{
			// TODO: number of points should be given by caller
			int pointsCount = 600;

			// Get highest/lowest frequencies in the circuit
			var lowestFrequency = factory.LowestFrequency;

			// Time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, lowestFrequency);

			// Get nodes constructed on the basis of the circuit
			var nodeIndices = factory.GetNodeIndicesWithoutReference().ToList();

			var usedSources = (includeDCBias ? factory.AllSources : factory.ACSources).Concat(a.Singleton);

			var adjustedState = new WaveformPartialState(factory.NodesCount, factory.ActiveComponentsCount, usedSources);

			// Potentials as waveforms with values adjusted for proper op-amp operation at each point
			var adjustedWaveforms = new Dictionary<int, IList<double>>[factory.ACVoltageSourcesCount];

			// Active components currents with values adjusted for proper op-amp operation at each point
			var adjustedActiveComponentsCurrents = new Dictionary<int, IList<double>>[factory.ACVoltageSourcesCount];

			// Waveforms of DC offset that are result of adjusted op-amp operation
			var adjustedDCOffsets = new Dictionary<int, IList<double>>();

			// Waveforms of DC active component currents that are results of adjusted op-amp operation
			var adjustedDCActiveComponentsCurrents = new Dictionary<int, IList<double>>();

			// Make an entry for each adjusted DC offset
			foreach(var index in nodeIndices)
			{
				adjustedDCOffsets.Add(index, new List<double>());
			}

			// Make an entry for each adjusted DC active component current
			for(int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				adjustedDCActiveComponentsCurrents.Add(i, new List<double>());
			}

			// For each AC voltage source
			for(int i = 0; i < factory.ACVoltageSourcesCount; ++i)
			{
				// Make entries in i-th entry in arrays
				adjustedWaveforms[i] = new Dictionary<int, IList<double>>();
				adjustedActiveComponentsCurrents[i] = new Dictionary<int, IList<double>>();

				// Create time domain signal for each node
				foreach (var index in nodeIndices)
				{
					adjustedWaveforms[i].Add(index, new List<double>());
				}

				// Create time domain signals for each active component current
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					adjustedActiveComponentsCurrents[i].Add(j, new List<double>());
				}
			}

			// Go through each simulation point separately - it is necessary in order to correctly determine op-amp operation at each specific point
			for (int i = 0; i < pointsCount; ++i)
			{
				// Use the helper to obtain instantenous potentials
				var instantenousValues = FullCycleInstantenousValuesHelper(factory, i, timeStep, includeDCBias);

				// Loop until correct op-amp operation is found
				while (!factory.CheckOpAmpOperationWithSelfAdjustment(instantenousValues.Combine().Potentials))
				{
					// If the op-amp operation was adjusted, recalculate the waveforms
					instantenousValues = FullCycleInstantenousValuesHelper(factory, i, timeStep, includeDCBias);
				}

				foreach(var state in instantenousValues.States.Values)
				{
					foreach (var index in nodeIndices)
					{
						// Lists are empty - points should just be added to them to form a full waveform
						adjustedState.States[state.SourceDescription].Potentials[index].Add(
							instantenousValues.States[state.SourceDescription].Potentials[index]);
					}
					
					// Add the instantenous values of active components currents to the waveforms
					foreach(var index in factory.ActiveComponentsIndices)
					{
						adjustedState.States[state.SourceDescription].Currents[index].Add(
							instantenousValues.States[state.SourceDescription].Currents[index]);
					}
				}

				// Finally reset the op-amp operation for next iteration
				factory.ResetOpAmpOperation();
			}

			// Dictionary holding final values of potentials at nodes that will be passed to simulation results
			var finalPotentials = new Dictionary<INode, ITimeDomainSignalMutable>()
			{
				// Add entry for reference node
				{ factory.GetNodes().First(), IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep) },
			};

			// Dictionary for final active components currents
			var finalActiveComponentsCurrents = new Dictionary<int, ITimeDomainSignalMutable>();

			// Get nodes in order to construct ITimeDomainSignals for them and pass those as pairs to simulation results
			var nodes = factory.GetNodesWithoutReference().ToList();

			// For each node index
			foreach(var index in nodeIndices)
			{
				// Create an ITimeDomainSignal
				finalPotentials.Add(nodes[index], IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep));
			}

			// For each active component
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				// Create an ITimeDomainSignal
				finalActiveComponentsCurrents.Add(i, IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep));
			}


			// Full waveforms were determined - assign them to final signals, go through each AC voltage source
			foreach(var source in usedSources)
			{
				// For each node
				foreach(var nodeIndex in factory.GetNodeIndicesWithoutReference())
				{
					// Fetch it
					var currentNode = nodes[nodeIndex];

					// And add a waveform generated by i-th AC voltage source on that node
					finalPotentials[currentNode].AddWaveform(source, adjustedState.States[source].Potentials[currentNode.Index]);
				}

				// For each active component
				foreach(var activeComponentIndex in factory.ActiveComponentsIndices)
				{
					// Add a waveform generated by i-th AC voltage source in this current
					finalActiveComponentsCurrents[activeComponentIndex].AddWaveform(
						source, adjustedState.States[source].Currents[activeComponentIndex]);
				}
			}

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				finalPotentials.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
				finalActiveComponentsCurrents.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
				timeStep,
				0);
		}

		#endregion

		#region Simulation wrapper

		/// <summary>
		/// Measured time, invokes simulation completed event and logs simulation completed event.
		/// Simulation completed message: "Calculated *simulation name* simulation in *measured time*ms".
		/// </summary>
		/// <param name="schematic">Schematic for which to perform the simulation</param>
		/// <param name="simulationLogic">Action that is responsible for performing and saving simulation results</param>
		/// <param name="simulationName">Name to use in simulation ended message</param>
		/// <param name="simulationType">Type of the simulation, for simulation ended event purposes</param>
		private void SimulationRunWrapper(ISchematic schematic, Action<AdmittanceMatrixFactory> simulationLogic, string simulationName,
			SimulationType simulationType)
		{
			// Create a stopwatch to measure the duration of simulation
			Stopwatch watch = new Stopwatch();

			// Start measuring time
			watch.Start();

			// Create an admittance matrix factory and use it to invoke passed simulation logic
			simulationLogic(new AdmittanceMatrixFactory(schematic));

			// Stop time measurement
			watch.Stop();

			// Invoke simulation completed event
			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(simulationType));

			// Log the simulation time
			IoC.Log("Completed " + simulationName + $" in {watch.ElapsedMilliseconds}ms",
				InfoLoggerMessageDuration.Short);
		}

		#endregion

		#endregion

		#region Public methods

		/// <summary>
		/// Performs a DC bias simulation of the circuit.
		/// </summary>
		/// <param name="schematic"></param>
		public void DCBias(ISchematic schematic) => SimulationRunWrapper(schematic, DCBiasLogic, "DC bias", SimulationType.DC);

		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="schematic"/>
		/// </summary>
		/// <param name="schematic"></param>
		public void ACFullCycleWithoutOpAmpAdjustment(ISchematic schematic) =>
			SimulationRunWrapper(schematic, (x) => FullCycleWithoutOpAmpAdjustment(x, false), "AC cycle without op-amp adjustment", SimulationType.AC);

		/// <summary>
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		public void ACDCFullCycleWithoutOpAmpAdjustment(ISchematic schematic) =>
			SimulationRunWrapper(schematic, (x) => FullCycleWithoutOpAmpAdjustment(x, true), "ACDC cycle without op-amp adjustment", SimulationType.ACDC);

		/// <summary>
		/// Performs a full cycle AC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		public void ACFullCycleWithOpAmpAdjustment(ISchematic schematic) =>
			SimulationRunWrapper(schematic, (x) => FullCycleLogicWithOpAmpAdjustment(x, false), "AC Cycle with op-amp adjustment", SimulationType.AC);

		/// <summary>
		/// Performs a full ACDC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		public void ACDCFullCycleWithOpAmpAdjustment(ISchematic schematic) =>
			SimulationRunWrapper(schematic, (x) => FullCycleLogicWithOpAmpAdjustment(x, true), "ACDC Cycle with op-amp adjustment", SimulationType.AC);

		#endregion

		#region Public static properties

		/// <summary>
		/// The index assumed for ground nodes
		/// </summary>
		public static int GroundNodeIndex { get; } = -1;

		#endregion
	}
}