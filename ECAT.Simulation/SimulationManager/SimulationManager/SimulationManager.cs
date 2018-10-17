using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

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

		#region Public methods

		/// <summary>
		/// Performs a single AC sweep for the given schematic
		/// </summary>
		/// <param name="schematic"></param>
		public void Bias(ISchematic schematic, SimulationType simulationType)
		{
			// Create a stopwatch to measure the duration of procedures 
			Stopwatch watch = new Stopwatch();

			// Start it for admittance matrix creation
			watch.Start();

			// Create an admittance matrix
			if (!AdmittanceMatrix.Construct(schematic, out var admittanceMatrix, out var error))
			{
				IoC.Log(error, InfoLoggerMessageDuration.Short);
				return;
			}

			try
			{
				// Solve it (for now try-catch for debugging)
				admittanceMatrix.Bias(simulationType, out var nodePotentials, out var activeComponentsCurrents);

				IoC.Log($"Calcualted {simulationType.ToString()} simulation in {watch.ElapsedMilliseconds}ms",
					InfoLoggerMessageDuration.Short);

				IoC.Resolve<SimulationResultsProvider>().Value =
					new SimulationResultsBias(nodePotentials, activeComponentsCurrents);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}

			watch.Reset();

			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(simulationType));
		}

		/// <summary>
		/// Performs a full cycle AC simulation
		/// </summary>
		/// <param name="schematic"></param>
		public void ACFullCycle(ISchematic schematic)
		{
			// Create a stopwatch to measure the duration of procedures 
			Stopwatch watch = new Stopwatch();

			// Start it for admittance matrix creation
			watch.Start();

			// Create an admittance matrix
			if (!AdmittanceMatrix.Construct(schematic, out var admittanceMatrix, out var error))
			{
				IoC.Log(error, InfoLoggerMessageDuration.Short);
				return;
			}

			try
			{
				// Solve it (for now try-catch for debugging)
				admittanceMatrix.ACCycle(out var nodePotentials, out var activeComponentsCurrents);

				IoC.Log($"Calcualted AC Cycle simulation in {watch.ElapsedMilliseconds}ms",
					InfoLoggerMessageDuration.Short);

				IoC.Resolve<SimulationResultsProvider>().Value =
					new SimulationResultsTime(nodePotentials, activeComponentsCurrents, 5e-3, 0);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}

			watch.Reset();

			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(SimulationType.ACDC));

			return;
			// Mock results for now

			var n1signal = IoC.Resolve<ITimeDomainSignalMutable>(100, 1d);
			var n2signal = IoC.Resolve<ITimeDomainSignalMutable>(100, 1d);
			n1signal.AddWaveform(5, (100).ToSequence().Select((x) => 0d));
			n2signal.AddWaveform(5, (100).ToSequence().Select((x) => Math.Sin(Math.PI * x / 10)));

			var mockResults = new KeyValuePair<INode, ITimeDomainSignal>[]
			{				
				new KeyValuePair<INode, ITimeDomainSignal>(new Node(){Index = 0 }, n1signal),
				new KeyValuePair<INode, ITimeDomainSignal>(new Node(){Index = 1 }, n2signal),
			} as IEnumerable<KeyValuePair<INode, ITimeDomainSignal>>;

			IoC.Resolve<SimulationResultsProvider>().Value =
				new SimulationResultsTime(mockResults,
				Enumerable.Empty<KeyValuePair<int, ITimeDomainSignal>>(), 1d, 0d);

			(schematic.Components.First((x) => x is IResistor) as IResistor).TerminalA.NodeIndex = 1;

			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(SimulationType.ACDC));
		}

		#endregion

		#region Public static properties

		/// <summary>
		/// The index assumed for ground nodes
		/// </summary>
		public static int GroundNodeIndex { get; } = 0;

		#endregion
	}
}