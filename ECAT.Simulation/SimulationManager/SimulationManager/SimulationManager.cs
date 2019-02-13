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

		#region Wave building

		private double GetSineWaveInstantenousValue(double amplitude, double frequency, double phaseShift, int pointIndex,
			double argumentStep, double constantOffset = 0)
		{
			// Return A * sin(2pi*f*arg + phi) + B
			return amplitude * Math.Sin(2 * Math.PI * frequency * pointIndex * argumentStep + phaseShift) + constantOffset;
		}

		/// <summary>
		/// Builds a sine wave based on given input parameters. X = A * sin(2pi * f * t + phi)
		/// </summary>
		/// <param name="amplitude">Amplitude of the wave (A)</param>
		/// <param name="frequency">Frequency of the wave (f)</param>
		/// <param name="phaseShift">Phase shift of the wave (phi)</param>
		/// <param name="numberOfPoints">Number of points constructed</param>
		/// <param name="argumentStep">Step with which argument (time) is increased with each point</param>
		/// <returns></returns>
		private IEnumerable<double> BuildSineWave(double amplitude, double frequency, double phaseShift, int numberOfPoints, double argumentStep,
			double constantOffset = 0)
		{
			double argument = 0;

			for (int i = 0; i < numberOfPoints; ++i)
			{
				// Return A * sin(2pi*f*t + phi) + B
				yield return amplitude * Math.Sin(2 * Math.PI * frequency * argument + phaseShift) + constantOffset;
				argument += argumentStep;
			}
		}

		/// <summary>
		/// Returns a constant wave equal to 0.
		/// </summary>
		/// <param name="numberOfPoints">Number of points constructed</param>
		/// <returns></returns>
		private IEnumerable<double> BuildZeroWave(int numberOfPoints)
		{
			for (int i = 0; i < numberOfPoints; ++i)
			{
				yield return 0;
			}
		}

		#endregion

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
				var keyedNodePotentials = factory.GetNodesWithoutReference().MergeSelect(
					nodePotentials, (x, y) => new KeyValuePair<INode, double>(x, y));

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
				totalPotentials[nodes.First()].AddWaveform(frequency, BuildZeroWave(pointsCount));
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
					totalPotentials[currentNode].AddWaveform(factory.GetACVoltageSourceFrequency(i), BuildSineWave(
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
					totalActiveComponentsCurrents[j].AddWaveform(factory.GetACVoltageSourceFrequency(i), BuildSineWave(
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
		/// <paramref name="pointIndex"/> and <paramref name="timeStep"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="totalPotentials">Calculated instantenous values of potentials at nodes</param>
		/// <param name="totalActiveComponentsCurrents">Calculated instantenous values of active components' currents</param>
		/// <param name="pointIndex">Index of the point for which to calculate instantenous values</param>
		/// <param name="timeStep">Time step between two calculational points</param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results, if false only saturated <see cref="IOpAmp"/>s
		/// will be considered for DC part of simulation</param>
		private void FullCycleInstantenousValuesHelper(AdmittanceMatrixFactory factory, out Dictionary<INode, double> totalPotentials,
			out Dictionary<int, double> totalActiveComponentsCurrents, int pointIndex, double timeStep, bool includeDCBias = false)
		{
			// Create dictionary holding final values of instantenous potentials at nodes
			totalPotentials = new Dictionary<INode, double>();

			// Create dictionary holding final values of instantenous active components' currents
			totalActiveComponentsCurrents = new Dictionary<int, double>();

			// Get nodes constructed on the basis of the circuit
			var nodes = factory.GetNodes();
			// Get nodes without the reference node and group them into a list for easier access with indexes
			var nodesWithoutReference = factory.GetNodesWithoutReference().ToList();

			// Create entry for potential at each node
			foreach (var node in nodes)
			{
				totalPotentials.Add(node, 0);
			}

			// Create entry for each active component current
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				totalActiveComponentsCurrents.Add(i, 0);
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

					// Add to appropriate node's instantenous potential calculated for i-th voltage source
					totalPotentials[currentNode] += GetSineWaveInstantenousValue(
						// Amplitude is the amplitude of the source times magnitude of transfer function
						factory.GetACVoltageSourceAmplitude(i) * nodePotentials[i, j].Magnitude,
						// Frequency of the i-th voltage source
						factory.GetACVoltageSourceFrequency(i),
						// Phase introduced by transfer function
						nodePotentials[i, j].Phase,
						// Index specified by caller
						pointIndex,
						// Time step specified by caller
						timeStep);
				}

				// Transfer functions for active components currents
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					// Add to appropriate current's instantenous value
					totalActiveComponentsCurrents[j] += GetSineWaveInstantenousValue(
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

			// Add the calculated DC potential at each node to corresponding nodes' waveforms, as constant value 
			for (int i = 0; i < nodes.Count() - 1; ++i)
			{
				// Get i-th node
				var currentNode = nodesWithoutReference[i];

				// Add the DC potential as constant value - take the real part because for DC bias results can only be purely real
				totalPotentials[currentNode] += dcPotentials[i].Real;
			}

			// Add the calculated DC active component currents to corresponding active components currents, as constant value
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				// Take the real part only because for DC bias results can only be purely real
				totalActiveComponentsCurrents[i] += dcCurrents[i].Real;
			}
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
			var nodes = factory.GetNodes().ToList();

			// Potentials as waveforms with values adjusted for proper op-amp operation at each point
			var adjustedWaveforms = new Dictionary<INode, IList<double>>();

			// Active components currents with values adjusted for proper op-amp operation at each point
			var adjustedActiveComponentsCurrents = new Dictionary<int, IList<double>>();

			// Create time domain signal for each node
			foreach (var node in nodes)
			{
				adjustedWaveforms.Add(node, new List<double>());
			}

			// Create time domain signals for each active component current
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				adjustedActiveComponentsCurrents.Add(i, new List<double>());
			}

			// Go through each simulation point separately - it is necessary in order to correctly determine op-amp operation at each specific point
			for (int i = 0; i < pointsCount; ++i)
			{
				// Use the helper to obtain instantenous potentials
				FullCycleInstantenousValuesHelper(factory, out var potentials, out var activeComponentsCurrents, i, timeStep, includeDCBias);
				
				// Loop until correct op-amp operation is found
				while (!factory.CheckOpAmpOperationWithSelfAdjustment(potentials.ToDictionary((x) => x.Key, (x) => x.Value)))
				{
					// If the op-amp operation was adjusted, recalculate the waveforms
					FullCycleInstantenousValuesHelper(factory, out potentials, out activeComponentsCurrents, i, timeStep, includeDCBias);
				}

				// At this point op-amp operation is correct - assign the instantenous potentials and currents to the final waveform
				for (int j = 1; j < nodes.Count; ++j)
				{
					var currentNode = nodes[j];

					// Lists are empty - points should just be added to them to form a full waveform
					adjustedWaveforms[currentNode].Add(potentials[currentNode]);
				}

				// Add the instantenous values of active components currents to the waveforms
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					adjustedActiveComponentsCurrents[j].Add(activeComponentsCurrents[j]);
				}

				// Finally reset the op-amp operation for next iteration
				factory.ResetOpAmpOperation();
			}

			// Dictionary holding final values of potentials at nodes that will be passed to simulation results
			var finalPotentials = new Dictionary<INode, ITimeDomainSignalMutable>
			{
				// Add potential for the reference node
				{ nodes[0], IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep) }
			};

			// Full waveforms were determined - assign them to final signals
			for (int i = 1; i < nodes.Count; ++i)
			{
				// Get the i-th node
				var currentNode = nodes[i];

				// Create time domain signal
				var signal = IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep);

				// Add the calculated waveform to it
				signal.AddWaveform(lowestFrequency, adjustedWaveforms[currentNode]);

				// Add the signal with its node to the dictionary
				finalPotentials.Add(currentNode, signal);
			}

			// Dictionary for final active components currents
			var finalActiveComponentsCurrents = new Dictionary<int, ITimeDomainSignalMutable>();

			// For each active component
			for(int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				// Create a signal
				var signal = IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep);

				// Add the calculated wave to it
				signal.AddWaveform(lowestFrequency, adjustedActiveComponentsCurrents[i]);

				// Add the signal with its index to the dictionary
				finalActiveComponentsCurrents.Add(i, signal);
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
		public static int GroundNodeIndex { get; } = 0;

		#endregion
	}
}