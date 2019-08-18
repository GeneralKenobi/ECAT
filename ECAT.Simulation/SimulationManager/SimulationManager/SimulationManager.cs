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
		private PhasorPartialStates GetPhasorsForAllDCSources(AdmittanceMatrixFactory factory) => GetAllPhasorsHelper(factory, factory.DCSources);
		
		/// <summary>
		/// Creates and solves DC admittance matrix for saturated op-amps
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		private PhasorState GetOpAmpSaturationBias(AdmittanceMatrixFactory factory)
		{
			// Create an instantenous state based on node indices and active components indices in factory,
			// use the factory's op amp saturation source as source description
			var result = new PhasorState(factory.Nodes, factory.ActiveComponentsCount, factory.OpAmpSaturationSource);

			// Construct matrix for saturated op-amps and solve it
			factory.ConstructDCForSaturatedOpAmpsOnly().Solve(out var nodePotentials, out var activeComponentsCurrents);

			// Add the results from simulation to state
			result.AddValues(nodePotentials.ToArray(), activeComponentsCurrents.ToArray());

			return result;
		}

		#endregion

		#region DC biasing

		/// <summary>
		/// Topmost logic behind DC bias - calling it will result in simulation being performed and saved to <see cref="ISimulationResultsProvider"/>
		/// </summary>
		/// <param name="factory"></param>
		private void DCBiasLogic(AdmittanceMatrixFactory factory)
		{
			// Calculated state of the system
			var state = GetPhasorsForAllDCSources(factory);

			// Add op-amp saturation bias to complete the DC bias
			state.AddState(GetOpAmpSaturationBias(factory));

			// Loop until correct op-amp operation is found
			while (!factory.CheckOpAmpOperationWithSelfAdjustment(state.ToDC().Combine().Potentials))
			{
				// If the op-amp operation was adjusted, recalculate the state
				state = GetPhasorsForAllDCSources(factory);
				
				state.AddState(GetOpAmpSaturationBias(factory));
			}

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
		private IDictionary<int, IEnumerable<Complex>> FrequencySweepHelper(AdmittanceMatrixFactory factory, int pointsCount, double start, double step)
		{
			IDictionary<int, IList<Complex>> result = new Dictionary<int, IList<Complex>>();
			factory.Nodes.ForEach((x) => result.Add(x, new List<Complex>()));

			double exponent = start;

			for(int i = 0; i < pointsCount; ++i, exponent+=step)
			{
				factory.SweepSource.Frequency = Math.Pow(10, exponent);

				// Get AC transfer functions
				var state = GetPhasor(factory, factory.SweepSource.Description);

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
		
		/// <summary>
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="factory"/>.
		/// After determining the transfer functions calculates only one set of instantenous values for point described by
		/// <paramref name="pointIndex"/> and <paramref name="timeStep"/>.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="pointIndex">Index of the point for which to calculate instantenous values</param>
		/// <param name="timeStep">Time step between two calculational points</param>
		/// <param name="includeDCBias">If true DC bias will be performed and added to results</param>
		/// <param name="performSaturatedOpAmpBias">If true, op-amp saturation bias will be performed and added to results</param>
		private InstantenousPartialStates FullCycleInstantenousValuesHelper(AdmittanceMatrixFactory factory, int pointIndex, double timeStep,
			bool includeDCBias = false, bool performSaturatedOpAmpBias = true)
		{
			// Get phasors for AC sources
			var phasors = GetPhasorsForAllACSources(factory);

			if(includeDCBias)
			{
				// Add DC phasors, if requested
				phasors.MergeWith(GetPhasorsForAllDCSources(factory));
			}

			// Transform those phasors to instantenous values
			var instantenous = phasors.ToInstantenousValue(pointIndex, timeStep);

			// Finally get op amp bias, if requested
			if (performSaturatedOpAmpBias)
			{
				instantenous.AddState(GetOpAmpSaturationBias(factory).ToDC());
			}

			return instantenous;
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
		private void FullCycleWithoutOpAmpAdjustment(AdmittanceMatrixFactory factory, bool includeDCBias)
		{
			// TODO: number of points should be given by caller
			int pointsCount = IoC.Resolve<IDefaultValues>().DefaultACCyclePointsCount;

			// Calculate time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, factory.LowestFrequency);

			// Use helper to get the state of the system
			var state = FullCycleHelper(factory, pointsCount, timeStep, includeDCBias);

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				// Transform potentials to time domain signals - specify that reference node should be added
				state.PotentialsToTimeDomainSignals(timeStep, true),
				state.CurrentsToTimeDomainSignals(timeStep),
				timeStep,
				0);
		}

		#endregion

		#region AC (DC) full cycle logic with op-amp adjustment

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
			// TODO: Try to make it so that saturated op amp bias is not considered to be pure DC but rather an extension of all sources
			// contributing to the circuit

			// TODO: number of points should be given by caller
			int pointsCount = IoC.Resolve<IDefaultValues>().DefaultACCyclePointsCount;

			// Time step between two subsequent points in time vector
			double timeStep = GetTimeStep(pointsCount, factory.LowestFrequency);

			// Create an enumeration of used sources - take AC sources and op-amp saturation source. If DC bias is requested, add the DC sources too.
			var usedSources = (includeDCBias ? factory.AllSources : factory.ACSources).Concat(factory.OpAmpSaturationSource);

			// Get node indices constructed on the basis of the circuit
			var nodeIndices = factory.Nodes.ToList();
			
			// Create a state which will be filled with adjusted simulation points
			var adjustedState = new WaveformPartialState(factory.NodesCount, factory.ActiveComponentsCount, usedSources);
			
			// Go through each simulation point separately - it is necessary in order to correctly determine op-amp operation at each specific point
			for (int i = 0; i < pointsCount; ++i)
			{
				// Use the helper to obtain instantenous potentials
				var instantenousValues = FullCycleInstantenousValuesHelper(factory, i, timeStep, includeDCBias);

				// Loop until correct op-amp operation is found
				while (!factory.CheckOpAmpOperationWithSelfAdjustment(instantenousValues.Combine().Potentials))
				{
					// If the op-amp operation was adjusted, recalculate the instantenous values
					instantenousValues = FullCycleInstantenousValuesHelper(factory, i, timeStep, includeDCBias);
				}

				// For each state
				foreach(var state in instantenousValues.States.Values)
				{
					// And for each node
					foreach (var index in nodeIndices)
					{
						// Add the calculated potential for state at node 'index' to the adjusted result waveform.
						// Lists are empty - points should just be added to them to form a full waveform.
						adjustedState.States[state.SourceDescription].Potentials[index].Add(
							instantenousValues.States[state.SourceDescription].Potentials[index]);
					}
					
					// Add the instantenous values of active components currents to the waveforms - just like currents
					foreach(var index in factory.ActiveComponents)
					{
						adjustedState.States[state.SourceDescription].Currents[index].Add(
							instantenousValues.States[state.SourceDescription].Currents[index]);
					}
				}

				// Finally reset the op-amp operation for next iteration
				factory.ResetOpAmpOperation();
			}

			// Create simulation results
			IoC.Resolve<SimulationResultsProvider>().Value = new SimulationResultsTime(
				// Convert the adjusted state to potentials - specify that reference node should be added
				adjustedState.PotentialsToTimeDomainSignals(timeStep, true),
				adjustedState.CurrentsToTimeDomainSignals(timeStep),
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
		private void FrequencySweep(AdmittanceMatrixFactory factory)
		{
			double startFrequency = Math.Log10(factory.SweepSource.StartFrequency);
			double endFrequency = Math.Log10(factory.SweepSource.EndFrequency);

			int pointsCount = IoC.Resolve<IDefaultValues>().DefaultFrequencySweepPointsCount;
			double step = (endFrequency - startFrequency) / (pointsCount - 1);

			// Use helper to get the state of the system
			var potentials = FrequencySweepHelper(factory, pointsCount, startFrequency, step);
			var signals = potentials.ToDictionary((x) => x.Key, (x) => IoC.Resolve<IFrequencyDomainSignal>(x.Value, step, startFrequency));
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
		private void SimulationRunWrapper(ISchematic schematic, Action<AdmittanceMatrixFactory> simulationLogic, string simulationName,
			SimulationType simulationType)
		{
			// Create a stopwatch to measure the duration of simulation
			Stopwatch watch = new Stopwatch();

			// Start measuring time
			watch.Start();

			try
			{
				// Create an admittance matrix factory
				var factory = new AdmittanceMatrixFactory(schematic);

				// Use it to invoke passed simulation logic
				simulationLogic(factory);

				// Assign voltmeters - if it's an AC simulation.
				IoC.Resolve<SimulationResultsProvider>().DeclaredVoltmeterMeasurements =
					simulationType == SimulationType.DC ? Enumerable.Empty<IVoltmeterMeasurement>() : factory.Voltmeters;
			}
			catch(Exception e)
			{
				IoC.Log("Exception thrown");
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
		/// Performs an AC cycle simulation - simulation is running for one full period of lowest frequency source in the <paramref name="schematic"/>
		/// </summary>
		/// <param name="schematic"></param>
		public void ACFullCycleWithoutOpAmpAdjustment(ISchematic schematic)
		{
			if(CheckSchematicForNormalSimulation(schematic))
			{
				SimulationRunWrapper(schematic, (x) => FullCycleWithoutOpAmpAdjustment(x, false), "AC cycle without op-amp adjustment", SimulationType.AC);
			}
		}

		/// <summary>
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		public void ACDCFullCycleWithoutOpAmpAdjustment(ISchematic schematic)
		{
			if(CheckSchematicForNormalSimulation(schematic))
			{
				SimulationRunWrapper(schematic, (x) => FullCycleWithoutOpAmpAdjustment(x, true), "ACDC cycle without op-amp adjustment", SimulationType.ACDC);
			}
		}

		/// <summary>
		/// Performs a full cycle AC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		public void ACFullCycleWithOpAmpAdjustment(ISchematic schematic)
		{
			if(CheckSchematicForNormalSimulation(schematic))
			{
				SimulationRunWrapper(schematic, (x) => FullCycleLogicWithOpAmpAdjustment(x, false), "AC Cycle with op-amp adjustment", SimulationType.AC);
			}
		}

		/// <summary>
		/// Performs a full ACDC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		public void ACDCFullCycleWithOpAmpAdjustment(ISchematic schematic)
		{
			if(CheckSchematicForNormalSimulation(schematic))
			{
				SimulationRunWrapper(schematic, (x) => FullCycleLogicWithOpAmpAdjustment(x, true), "ACDC Cycle with op-amp adjustment", SimulationType.AC);
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