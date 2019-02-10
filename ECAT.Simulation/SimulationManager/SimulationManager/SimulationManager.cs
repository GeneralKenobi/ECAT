using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
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

		/// <summary>
		/// Builds a sine wave based on given input parameters. X = A * sin(2pi * f * t + phi)
		/// </summary>
		/// <param name="amplitude">Amplitude of the wave (A)</param>
		/// <param name="frequency">Frequency of the wave (f)</param>
		/// <param name="phaseShift">Phase shift of the wave (phi)</param>
		/// <param name="numberOfPoints">Number of points constructed</param>
		/// <param name="argumentStep">Step with which argument (time) is increased with each point</param>
		/// <returns></returns>
		private IEnumerable<double> BuildSineWave(double amplitude, double frequency, double phaseShift, int numberOfPoints, double argumentStep)
		{
			double argument = 0;

			for (int i = 0; i < numberOfPoints; ++i)
			{
				// Return A * sin(2pi*f*t + phi)
				yield return amplitude * Math.Sin(2 * Math.PI * frequency * argument + phaseShift);
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

			// Solve it
			DCBiasHelper(factory, out var nodePotentials, out var activeComponentsCurrents);

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

		#region AC full cycle

		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="factory"/>.
		/// Returns calculated node potentials and active component currents as sinusoidal waveforms.
		/// </summary>
		/// <param name="schematic"></param>
		private void ACFullCycleHelper(AdmittanceMatrixFactory factory, out Dictionary<INode, ITimeDomainSignalMutable> totalPotentials,
			out Dictionary<int, ITimeDomainSignalMutable> totalActiveComponentsCurrents, out double timeStep)
		{
			// Number of points per one cycle of highest frequency signal
			int pointsPerHighestFrequency = 200;

			// Get highest/lowest frequencies in the circuit
			var highestFrequency = factory.FrequenciesInCircuit.Max();
			var lowestFrequency = factory.FrequenciesInCircuit.Min();

			// The last calculational point 
			double endTime = 1 / lowestFrequency;

			// Number of points in time vector
			int timeVectorPointsCount = (int)Math.Ceiling(highestFrequency / lowestFrequency * pointsPerHighestFrequency);

			// Time step between two subsequent points in time vector
			timeStep = endTime / timeVectorPointsCount;

			// Create dictionary holding final values of potentials at nodes
			totalPotentials = new Dictionary<INode, ITimeDomainSignalMutable>();

			// Create dictionary holding final values of potentials at nodes
			totalActiveComponentsCurrents = new Dictionary<int, ITimeDomainSignalMutable>();

			// Get nodes constructed on the basis of the circuit
			var nodes = factory.GetNodes();

			// Create time domain signal for each node
			foreach (var node in nodes)
			{
				totalPotentials.Add(node, IoC.Resolve<ITimeDomainSignalMutable>(timeVectorPointsCount, timeStep));
			}

			// Create time domain signals for each active component current
			for (int i = 0; i < factory.ActiveComponentsCount; ++i)
			{
				totalActiveComponentsCurrents.Add(i, IoC.Resolve<ITimeDomainSignalMutable>(timeVectorPointsCount, timeStep));
			}

			// Add zero wave to reference node
			totalPotentials[nodes.First()].AddWaveform(0, BuildZeroWave(timeVectorPointsCount));

			// Get all transfer functions
			GetAllACTransferFunctions(factory, out var nodePotentials, out var activeComponentsCurrents);

			// For each source
			for (int i = 0; i < factory.ACVoltageSourcesCount; ++i)
			{
				// Get an enumerator to nodes and move it to first element (iteration should skip it - it's the reference node)
				var it = nodes.GetEnumerator();
				it.MoveNext();

				// For each node transfer function for i-th source
				for (int j = 0; j < nodePotentials.GetLength(1); ++j)
				{
					// Advance the iterator
					it.MoveNext();

					// Add the waveform to appropriate node's time signal
					totalPotentials[it.Current].AddWaveform(factory.GetACVoltageSourceFrequency(i), BuildSineWave(
						factory.GetACVoltageSourceAmplitude(i) * nodePotentials[i, j].Magnitude,
						factory.GetACVoltageSourceFrequency(i),
						nodePotentials[i, j].Phase,
						timeVectorPointsCount,
						timeStep));
				}

				// For each active component current transfer function for i-th source
				for (int j = 0; j < factory.ActiveComponentsCount; ++j)
				{
					totalActiveComponentsCurrents[j].AddWaveform(factory.GetACVoltageSourceFrequency(i), BuildSineWave(
						factory.GetACVoltageSourceAmplitude(i) * activeComponentsCurrents[i, j].Magnitude,
						factory.GetACVoltageSourceFrequency(i),
						activeComponentsCurrents[i, j].Phase,
						timeVectorPointsCount,
						timeStep));
				}
			}
		}

		/// <summary>
		/// Topmost logic behind AC Full Cycle - calling it will result in simulation being performed and saved to
		/// <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		private void ACFullCycleLogic(AdmittanceMatrixFactory factory)
		{
			ACFullCycleHelper(factory, out var totalPotentials, out var totalActiveComponentsCurrents, out var timeStep);

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				totalPotentials.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
				totalActiveComponentsCurrents.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
				timeStep,
				0);
		}

		#endregion

		#region ACDC full cycle

		/// <summary>
		/// Topmost logic behind AC Full Cycle - calling it will result in simulation being performed and saved to
		/// <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		private void ACDCFullCycleLogic(AdmittanceMatrixFactory factory)
		{
			// Get AC simulation results
			ACFullCycleHelper(factory, out var totalPotentials, out var totalActiveComponentsCurrents, out var timeStep);

			// Get DC simulation results
			DCBiasHelper(factory, out var dcNodePotentials, out var dcActiveComponentsCurrents);

			// Add the DC offsets to AC waveforms
			var enumerator = factory.GetNodes().GetEnumerator();
			enumerator.MoveNext();

			for(int i = 0; i < dcNodePotentials.Count(); ++i)
			{
				enumerator.MoveNext();
				totalPotentials[enumerator.Current].AddConstantOffset(dcNodePotentials[i]);
			}

			for(int i = 0; i < dcActiveComponentsCurrents.Count(); ++i)
			{
				totalActiveComponentsCurrents[i].AddConstantOffset(dcActiveComponentsCurrents[i]);
			}

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				totalPotentials.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
				totalActiveComponentsCurrents.ToDictionary((x) => x.Key, (x) => (ITimeDomainSignal)x.Value),
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
		public void ACFullCycle(ISchematic schematic) => SimulationRunWrapper(schematic, ACFullCycleLogic, "AC Cycle", SimulationType.AC);

		/// <summary>
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		public void ACDCFullCycle(ISchematic schematic) => SimulationRunWrapper(schematic, ACDCFullCycleLogic, "ACDC Cycle", SimulationType.ACDC);


		#endregion

		#region Public static properties

		/// <summary>
		/// The index assumed for ground nodes
		/// </summary>
		public static int GroundNodeIndex { get; } = 0;

		#endregion
	}
}