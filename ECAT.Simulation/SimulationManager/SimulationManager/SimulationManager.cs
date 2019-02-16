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
		/// Helper function performing dc bias simulation for <see cref="ISchematic"/> which was used to construct
		/// <see cref="AdmittanceMatrixFactory"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="nodePotentials"></param>
		/// <param name="activeComponentsCurrents"></param>
		private void DCBiasHelper(AdmittanceMatrixFactory factory, out double[] nodePotentials, out double[] activeComponentsCurrents)
		{
			// Create an admittance matrix corresponding to it
			var matrix = factory.ConstructDC();

			// Solve it
			matrix.Solve(out var calculatedNodePotentials, out var calculatedActiveComponentsCurrents);

			// Use the Real part of the results only - DC matrices can't produce imaginary parts not equal to 0.
			nodePotentials = calculatedNodePotentials.
				Select((x) => x.Real).
				ToArray();

			activeComponentsCurrents = calculatedActiveComponentsCurrents.
				Select((x) => x.Real).
				ToArray();
		}

		/// <summary>
		/// Topmost logic behind DC bias - calling it will result in simulation being performed and saved to <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		private void DCBiasLogic(AdmittanceMatrixFactory factory)
		{
			// Create dictionary holding final values of potentials at nodes
			var totalPotentials = new Dictionary<INode, IPhasorDomainSignal>();

			// Create dictionary holding final values of potentials at nodes
			var totalActiveComponentsCurrents = new Dictionary<int, IPhasorDomainSignal>();

			// Get nodes constructed on the basis of the circuit
			var nodes = factory.GetNodes();

			bool opAmpOperationCorrect = true;

			// Variables for results
			double[] nodePotentials = null;
			double[] activeComponentsCurrents = null;

			// Loop that will find correct op-amp settings - initial settings are tested and, if not correct, adjusted. This process
			// goes on until the correct configuration is found.
			do
			{
				// Solve the admittance matrix
				DCBiasHelper(factory, out nodePotentials, out activeComponentsCurrents);

				// Assign node potentials to nodes
				var keyedNodePotentials = factory.GetSimulationNodeIndices().MergeSelect(
					nodePotentials, (x, y) => new KeyValuePair<int, double>(x, y));

				// Check if op-amps operate correctly with self-adjustment
				opAmpOperationCorrect = factory.CheckOpAmpOperationWithSelfAdjustment(keyedNodePotentials);
			}
			while (!opAmpOperationCorrect);

			// Get enumerator to nodes
			var enumerator = nodes.GetEnumerator();

			// Move to the first (reference) node
			enumerator.MoveNext();

			// Create signal for it
			totalPotentials.Add(enumerator.Current, IoC.Resolve<IPhasorDomainSignal>(0d));

			// For each calculated node potential
			for (int i = 0; i < nodePotentials.Count(); ++i)
			{
				// Move to the next node
				enumerator.MoveNext();

				// And create a phasor domain signal for it
				totalPotentials.Add(enumerator.Current, IoC.Resolve<IPhasorDomainSignal>(nodePotentials[i]));
			}

			// For each active component
			for (int i = 0; i < activeComponentsCurrents.Count(); ++i)
			{
				// Create a phasor domain signal for its current
				totalActiveComponentsCurrents.Add(i, IoC.Resolve<IPhasorDomainSignal>(activeComponentsCurrents[i]));
			}

			// Create simulation results based on determined node potentials and active components currents
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsBias(totalPotentials, totalActiveComponentsCurrents);
		}

		#endregion

		#region AC transfer functions

		/// <summary>
		/// Returns AC transfer functions constructed for voltage source given by <paramref name="sourceIndex"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="sourceIndex"></param>
		/// <param name="nodePotentialTransferFunctions"></param>
		/// <param name="activeComponentsCurrentsTransferFunctions"></param>
		private void GetACTransferFunction(AdmittanceMatrixFactory factory, int sourceIndex, out Complex[] nodePotentialTransferFunctions,
			out Complex[] activeComponentsCurrentsTransferFunctions)
		{
			// Create an admittance matrix corresponding to the given source
			var matrix = factory.ConstructAC(sourceIndex);

			// Solve it
			matrix.Solve(out nodePotentialTransferFunctions, out activeComponentsCurrentsTransferFunctions);
		}

		/// <summary>
		/// Returns an array of AC transfer functions - in total for all voltage sources found by <paramref name="factory"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="nodePotentialTransferFunctions"></param>
		/// <param name="activeComponentsCurrentsTransferFunctions"></param>
		private void GetAllACTransferFunctions(AdmittanceMatrixFactory factory, out Complex[,] nodePotentialTransferFunctions,
			out Complex[,] activeComponentsCurrentsTransferFunctions)
		{
			// Create result arrays
			nodePotentialTransferFunctions = new Complex[factory.ACVoltageSourcesCount, factory.NodesCount];
			activeComponentsCurrentsTransferFunctions = new Complex[factory.ACVoltageSourcesCount, factory.ActiveComponentsCount];

			// For each AC source
			for (int i = 0; i < factory.ACVoltageSourcesCount; ++i)
			{
				// Get transfer function
				GetACTransferFunction(factory, i, out var nodePotentials, out var activeComponentsCurrents);

				// Copy it into the result arrays
				nodePotentialTransferFunctions.CopyRowInto(nodePotentials, i);
				activeComponentsCurrentsTransferFunctions.CopyRowInto(activeComponentsCurrents, i);
			}
		}

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
		private void FullCycleHelper(AdmittanceMatrixFactory factory, out Dictionary<INode, ITimeDomainSignalMutable> totalPotentials,
			out Dictionary<int, ITimeDomainSignalMutable> totalActiveComponentsCurrents, int pointsCount, double timeStep, bool includeDCBias = false)
		{
			// Create dictionary holding final values of potentials at nodes
			totalPotentials = new Dictionary<INode, ITimeDomainSignalMutable>();

			// Create dictionary holding final values of potentials at nodes
			totalActiveComponentsCurrents = new Dictionary<int, ITimeDomainSignalMutable>();

			// Get nodes constructed on the basis of the circuit
			var nodes = factory.GetNodes();
			// Get nodes without the reference node and group them into a list for easier access with indexes
			var nodesWithoutReference = factory.GetNodesWithoutReference().ToList();

			// Create time domain signal for each node
			foreach (var node in nodes)
			{
				totalPotentials.Add(node, IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep));
			}

			// Create time domain signals for each active component current
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				totalActiveComponentsCurrents.Add(i, IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep));
			}

			// Add zero wave to reference node for each frequency in the circuit
			foreach (var frequency in factory.FrequenciesInCircuit)
			{
				totalPotentials[nodes.First()].AddWaveform(frequency, WaveformBuilder.ZeroWave(pointsCount));
			}

			// Get all transfer functions
			GetAllACTransferFunctions(factory, out var nodePotentials, out var activeComponentsCurrents);

			// Get DC transfer function - if DC biasing was specified construct a full DC admittance matrix, if not construct DC matrix only for
			// saturated op-amps, then solve the constructed matrix
			(includeDCBias ? factory.ConstructDC() : factory.ConstructDCForSaturatedOpAmpsOnly()).Solve(out var dcPotentials, out var dcCurrents);

			// For each function for i-th voltage source
			for (int i = 0; i < factory.ACVoltageSourcesCount; ++i)
			{
				// Transfer functions for node potentials
				for (int j = 0; j < nodePotentials.GetLength(1); ++j)
				{
					// Get j-th node
					var currentNode = nodesWithoutReference[j];

					// Add to appropriate node's potentials calculated for i-th voltage source
					totalPotentials[currentNode].AddWaveform(factory.GetACVoltageSourceFrequency(i), WaveformBuilder.SineWave(
						// Amplitude is the amplitude of the source times magnitude of transfer function
						factory.GetACVoltageSourceAmplitude(i) * nodePotentials[i, j].Magnitude,
						// Frequency of the i-th voltage source
						factory.GetACVoltageSourceFrequency(i),
						// Phase introduced by transfer function
						nodePotentials[i, j].Phase,
						// Number of points specified by caller
						pointsCount,
						// Time step specified by caller
						timeStep));
				}

				// For each active component current transfer function for i-th source
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					// Add to appropriate active component's current's calculated current for i-th voltage source
					totalActiveComponentsCurrents[j].AddWaveform(factory.GetACVoltageSourceFrequency(i), WaveformBuilder.SineWave(
						// Amplitude is the amplitude of the source times magnitude of transfer function
						factory.GetACVoltageSourceAmplitude(i) * activeComponentsCurrents[i, j].Magnitude,
						// Frequency of the i-th voltage source
						factory.GetACVoltageSourceFrequency(i),
						// Phase introduced by transfer function
						activeComponentsCurrents[i, j].Phase,
						// Number of points specified by caller
						pointsCount,
						// Time step specified by caller
						timeStep));
				}

			}

			// Add the calculated DC potential at each node to corresponding nodes' waveforms, as constant value 
			for (int i = 0; i < nodes.Count() - 1; ++i)
			{
				// Get i-th node
				var currentNode = nodesWithoutReference[i];

				// Add the DC potential as constant value - take the real part because for DC bias results can only be purely real
				totalPotentials[currentNode].AddConstantOffset(dcPotentials[i].Real);
			}

			// Add the calculated DC active component currents to corresponding active components currents, as constant value
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				// Take the real part only because for DC bias results can only be purely real
				totalActiveComponentsCurrents[i].AddConstantOffset(dcCurrents[i].Real);
			}
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
		private PartialInstantenousStates FullCycleInstantenousValuesHelper(AdmittanceMatrixFactory factory, int pointIndex, double timeStep,
			bool includeDCBias = false)
		{
			// Get indices of nodes, without the reference node, and group them into a list for easier access
			var nodeIndices = factory.GetSimulationNodeIndices().ToList();

			// Result container that will be returned
			var result = new PartialInstantenousStates(factory.ACVoltageSourcesCount, nodeIndices, factory.ActiveComponentsCount);
			
			// Get all transfer functions
			GetAllACTransferFunctions(factory, out var nodePotentials, out var activeComponentsCurrents);

			// For each function for i-th voltage source
			for (int i = 0; i < factory.ACVoltageSourcesCount; ++i)
			{
				// Transfer functions for node potentials
				foreach(var index in nodeIndices)
				{
					// Add to appropriate node's instantenous potential calculated for i-th voltage source
					result.ACStates[i].Potentials[index] += WaveformBuilder.SineWaveInstantenousValue(
						// Amplitude is the amplitude of the source times magnitude of transfer function
						factory.GetACVoltageSourceAmplitude(i) * nodePotentials[i, index].Magnitude,
						// Frequency of the i-th voltage source
						factory.GetACVoltageSourceFrequency(i),
						// Phase introduced by transfer function
						nodePotentials[i, index].Phase,
						// Index specified by caller
						pointIndex,
						// Time step specified by caller
						timeStep);
				}

				// Transfer functions for active components currents
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					// Add to appropriate current's instantenous value
					result.ACStates[i].Currents[j] += WaveformBuilder.SineWaveInstantenousValue(
						// Amplitude is the amplitude of the source times magnitude of transfer function
						factory.GetACVoltageSourceAmplitude(i) * activeComponentsCurrents[i, j].Magnitude,
						// Frequency of the i-th voltage source
						factory.GetACVoltageSourceFrequency(i),
						// Phase introduced by transfer function
						activeComponentsCurrents[i, j].Phase,
						// Index specified by caller
						pointIndex,
						// Time step specified by caller
						timeStep);
				}
			}

			// Get DC transfer function - if DC biasing was specified construct a full DC admittance matrix, if not construct DC matrix only for
			// saturated op-amps, then solve the constructed matrix
			(includeDCBias ?
				factory.ConstructDC() : factory.ConstructDCForSaturatedOpAmpsOnly()).Solve(out var dcBiasPotentials, out var dcBiasCurrents);

			// Cast the results to their real parts only - DC biasing can't produce complex values.
			// Transfer functions for node potentials
			foreach(var index in nodeIndices)
			{
				// Add to appropriate node's instantenous potential calculated for i-th voltage source
				result.DCState.Potentials[index] += dcBiasPotentials[index].Real;
			}

			// Transfer functions for active components currents
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				// Add to appropriate node's instantenous potential calculated for i-th voltage source
				result.DCState.Currents[i] += dcBiasCurrents[i].Real;
			}

			return result;
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
			var lowestFrequency = factory.FrequenciesInCircuit.Min();

			// Time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, lowestFrequency);

			FullCycleHelper(factory, out var totalPotentials, out var totalActiveComponentsCurrents, pointsCount, timeStep, includeDCBias);

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				totalPotentials.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
				totalActiveComponentsCurrents.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
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
			var lowestFrequency = factory.FrequenciesInCircuit.Min();

			// Time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, lowestFrequency);

			// Get nodes constructed on the basis of the circuit
			var nodeIndices = factory.GetSimulationNodeIndices().ToList();

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

				// At this point op-amp operation is correct - assign the instantenous potentials and currents to the final waveform
				for (int j = 0; j < factory.ACVoltageSourcesCount; ++j)
				{
					foreach(var index in nodeIndices)
					{
						// Lists are empty - points should just be added to them to form a full waveform
						adjustedWaveforms[j][index].Add(instantenousValues.ACStates[j].Potentials[index]);
					}

					// Add the instantenous values of active components currents to the waveforms
					for (int k = 0; k < factory.ActiveComponentsCount; ++k)
					{
						adjustedActiveComponentsCurrents[j][k].Add(instantenousValues.ACStates[j].Currents[k]);
					}
				}

				// For each node
				foreach(var index in nodeIndices)
				{
					// Assign DC potential to adjusted DC waveform
					adjustedDCOffsets[index].Add(instantenousValues.DCState.Potentials[index]);
				}

				// For each active component
				for(int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					// Assign its current
					adjustedDCActiveComponentsCurrents[j].Add(instantenousValues.DCState.Currents[j]);
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
			for (int i = 0; i < factory.ACVoltageSourcesCount; ++i)
			{
				// For each node
				for (int j = 0; j < nodes.Count; ++j)
				{
					// Fetch it
					var currentNode = nodes[j];

					// And add a waveform generated by i-th AC voltage source on that node
					finalPotentials[currentNode].AddWaveform(factory.GetACVoltageSourceFrequency(i), adjustedWaveforms[i][currentNode.Index]);
				}

				// For each active component
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					// Add a waveform generated by i-th AC voltage source in this current
					finalActiveComponentsCurrents[j].AddWaveform(factory.GetACVoltageSourceFrequency(i), adjustedActiveComponentsCurrents[i][j]);
				}
			}
						
			// Finally add DC waveforms to each node
			foreach(var index in nodeIndices)
			{
				finalPotentials[nodes[index]].AddDCWaveform(adjustedDCOffsets[index]);
			}

			// And DC currents to active components
			for(int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				finalActiveComponentsCurrents[i].AddDCWaveform(adjustedDCActiveComponentsCurrents[i]);
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