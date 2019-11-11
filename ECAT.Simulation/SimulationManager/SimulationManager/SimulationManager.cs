using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

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

		#region Admittance matrix solving for specific sources

		/// <summary>
		/// Returns phasors constructed for source given by <paramref name="sourceDescription"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="sourceDescription"></param>
		private PhasorState GetPhasor(AdmittanceMatrixFactory factory, ISourceDescription sourceDescription)
		{
			var state = new PhasorState(factory.NodesCount, factory.ActiveComponentsCount, sourceDescription);

			// Create an admittance matrix corresponding to the given source
			factory.Construct(sourceDescription).
				// And solve it
				Solve(out var nodePotentials, out var activeComponentsCurrents);

			// Add the results to state
			state.AddValues(nodePotentials, activeComponentsCurrents);

			return state;
		}

		/// <summary>
		/// Helper for functions that return phasors generated for many sources. Returns a <see cref="PhasorPartialStates"/> that contains
		/// phasors for all sources in <paramref name="sources"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="sources">Enumeration of sources to construct transfer functions for. All sources listed have to be present in
		/// <returns></returns>
		private PhasorPartialStates GetAllPhasorsHelper(AdmittanceMatrixFactory factory, IEnumerable<ISourceDescription> sources)
		{
			var result = new PhasorPartialStates(factory.NodesCount, factory.ActiveComponentsCount, sources);

			// For each source
			foreach (var source in sources)
			{
				// Get phasor and add it to states
				result.States[source] = GetPhasor(factory, source);
			}

			return result;
		}

		/// <summary>
		/// Returns phasors with for all AC sources in <paramref name="factory"/>
		/// </summary>
		/// <param name="factory"></param>
		private PhasorPartialStates GetPhasorsForAllACSources(AdmittanceMatrixFactory factory) =>
			GetAllPhasorsHelper(factory, factory.ACVoltageSources);

		/// <summary>
		/// Returns phasors with for all DC sources in <paramref name="factory"/>
		/// </summary>
		/// <param name="factory"></param>
		private PhasorPartialStates GetPhasorsForAllDCSources(AdmittanceMatrixFactory factory) =>
			GetAllPhasorsHelper(factory, factory.GetDCSources().Concat(factory.GetCurrentSources()));

		#endregion

		#region DC biasing

		/// <summary>
		/// Topmost logic behind DC bias - calling it will result in simulation being performed and saved to <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		private async Task DCBiasLogic(AdmittanceMatrixFactory factory)
		{
			PhasorPartialStates state = null;

			await Task.Run(() => state = GetPhasorsForAllDCSources(factory));

			// Create simulation results based on node potentials and active components currents in the state
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsBias(
				state.PotentialsToPhasorDomainSignal(true),
				state.CurrentsToPhasorDomainSignal());
		}

		#endregion

		#region Frequency sweep helper

		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="factory"/>.
		/// Returns calculated node potentials and active component currents as sinusoidal waveforms.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="pointsCount">Number of points to calculate for the signals</param>
		/// <param name="step">Time step between two calculational points</param>
		private async Task<IDictionary<int, IEnumerable<Complex>>> FrequencySweepHelper(AdmittanceMatrixFactory factory, int pointsCount, double start, double step)
		{
			IDictionary<int, IList<Complex>> result = new Dictionary<int, IList<Complex>>();
			factory.Nodes.ForEach((x) => result.Add(x, new List<Complex>()));

			double exponent = start;

			for (int i = 0; i < pointsCount; ++i, exponent += step)
			{
				factory.SweepSource.Frequency = Math.Pow(10, exponent);

				PhasorState state = null;
				await Task.Run(() => state = GetPhasor(factory, factory.SweepSource.Description));

				state.Potentials.ForEach((x) => result[x.Key].Add(x.Value));
			}

			return result.ToDictionary((x) => x.Key, (x) => x.Value as IEnumerable<Complex>);
		}

		#endregion

		#region AC full cycle helpers

		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="factory"/>.
		/// Returns calculated node potentials and active component currents as sinusoidal waveforms.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="pointsCount">Number of points to calculate for the signals</param>
		/// <param name="timeStep">Time step between two calculational points</param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results</param>
		private WaveformPartialState FullCycleHelper(AdmittanceMatrixFactory factory, int pointsCount, double timeStep, bool includeDCBias = false)
		{
			// Get AC transfer functions
			var systemState = GetPhasorsForAllACSources(factory);

			if (includeDCBias)
			{
				// As well as DC transfer function, if requested
				systemState.MergeWith(GetPhasorsForAllDCSources(factory));
			}

			return systemState.ToWaveform(pointsCount, timeStep);
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
			// Time step between two subsequent points in time vector - full period is 1 / lowestFrequency, time step is given by full period
			// divided by the number of points
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
		private async Task FullCycle(AdmittanceMatrixFactory factory, bool includeDCBias)
		{
			int pointsCount = IoC.Resolve<IDefaultValues>().DefaultACCyclePointsCount;
			// Calculate time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, factory.LowestFrequency);
			WaveformPartialState state = null;

			await Task.Run(() => state = FullCycleHelper(factory, pointsCount, timeStep, includeDCBias));

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				// Transform potentials to time domain signals - specify that reference node should be added
				state.PotentialsToTimeDomainSignals(timeStep, true),
				state.CurrentsToTimeDomainSignals(timeStep),
				timeStep,
				0);
		}

		#endregion

		#region Frequency Sweep

		/// <summary>
		/// Topmost logic behind AC Full Cycle - calling it will result in simulation being performed and saved to
		/// <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results, if false only saturated <see cref="IOpAmp"/>s
		/// will be considered for DC part of simulation</param>
		private async Task FrequencySweep(AdmittanceMatrixFactory factory)
		{
			double startFrequency = Math.Log10(factory.SweepSource.StartFrequency);
			double endFrequency = Math.Log10(factory.SweepSource.EndFrequency);

			int pointsCount = IoC.Resolve<IDefaultValues>().DefaultFrequencySweepPointsCount;
			double step = (endFrequency - startFrequency) / (pointsCount - 1);

			var potentials = FrequencySweepHelper(factory, pointsCount, startFrequency, step);
			var signals = (await potentials).ToDictionary((x) => x.Key, (x) => IoC.Resolve<IFrequencyDomainSignal>(x.Value, step, startFrequency));
			signals.Add(-1, IoC.Resolve<IFrequencyDomainSignal>(pointsCount, step, startFrequency));

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsFrequency(signals);
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
		private async Task SimulationRunWrapper(ISchematic schematic, Func<AdmittanceMatrixFactory, Task> simulationLogic, string simulationName,
			SimulationType simulationType)
		{
			// Create a stopwatch to measure the duration of simulation
			Stopwatch watch = new Stopwatch();

			IoC.Log("Starting " + simulationName + " simulation", InfoLoggerMessageDuration.Infinite);

			// Start measuring time
			watch.Start();

			try
			{
				// Create an admittance matrix factory
				var factory = new AdmittanceMatrixFactory(schematic);

				// Use it to invoke passed simulation logic
				await simulationLogic(factory);

				// Assign voltmeters - if it's an AC simulation.
				IoC.Resolve<SimulationResultsProvider>().DeclaredVoltmeterMeasurements =
					simulationType == SimulationType.DC ? Enumerable.Empty<IVoltmeterMeasurement>() : factory.Voltmeters;
			}
			catch(Exception)
			{
				IoC.Log($"There are errors in the schematic - please correct them and try again. Completed in {watch.ElapsedMilliseconds}ms");
				watch.Stop();
				return;
			}

			// Stop time measurement
			watch.Stop();

			// Invoke simulation completed event
			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(simulationType));

			// Log the simulation time
			IoC.Log("Completed " + simulationName + $" in {watch.ElapsedMilliseconds}ms",
				InfoLoggerMessageDuration.Short);
		}

		/// <summary>
		/// Checks if schematic passes all requirements necessary to perform a normal (i.e. not a sweep) simulation.
		/// If it doesn't then an exception is thrown.
		/// </summary>
		/// <param name="schematic"></param>
		private bool CheckSchematicForNormalSimulation(ISchematic schematic)
		{
			if (schematic.Components.Where((x) => x is ISweepVoltageSource).Count() != 0)
			{
				IoC.Log("No sweep voltage sources are allowed in normal simulation");
				return false;
			}

			if (schematic.Components.Where((x) => x is IDCVoltageSource || x is ICurrentSource || x is IACVoltageSource).Count() == 0)
			{
				IoC.Log("There are no sources in the system");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks if schematic passes all requirements necessary to perform a normal (i.e. not a sweep) simulation.
		/// If it doesn't then an exception is thrown.
		/// </summary>
		/// <param name="schematic"></param>
		private bool CheckSchematicForFrequencySweep(ISchematic schematic)
		{
			if (schematic.Components.Where((x) => x is ISweepVoltageSource).Count() != 1)
			{
				IoC.Log("There may be only 1 sweep source");
				return false;
			}

			if (schematic.Components.Where((x) => x is IDCVoltageSource || x is ICurrentSource || (x is IACVoltageSource && !(x is ISweepVoltageSource))).Count() > 0)
			{
				IoC.Log("During sweep there may be no other sources");
				return false;
			}

			return true;
		}

		#endregion

		#endregion

		#region Public methods

		/// <summary>
		/// Performs a DC bias simulation of the circuit.
		/// </summary>
		/// <param name="schematic"></param>
		public void DCBias(ISchematic schematic)
		{
			if(CheckSchematicForNormalSimulation(schematic))
			{
				SimulationRunWrapper(schematic, DCBiasLogic, "DC bias", SimulationType.DC);
			}
		}

		/// <summary>
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		public void ACDCFullCycle(ISchematic schematic)
		{
			if(CheckSchematicForNormalSimulation(schematic))
			{
				SimulationRunWrapper(schematic, (x) => FullCycle(x, true), "ACDC Full Cycle", SimulationType.ACDC);
			}
		}

		/// <summary>
		/// Performs a full ACDC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		public void FrequencySweep(ISchematic schematic)
		{
			if(CheckSchematicForFrequencySweep(schematic))
			{
				SimulationRunWrapper(schematic, (x) => FrequencySweep(x), "Frequency Sweep", SimulationType.AC);
			}
		}

		#endregion
	}
}