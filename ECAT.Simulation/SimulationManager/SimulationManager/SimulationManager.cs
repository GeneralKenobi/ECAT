using ECAT.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;

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
				admittanceMatrix.Bias(simulationType);

				IoC.Log($"Calcualted {simulationType.ToString()} simulation in {watch.ElapsedMilliseconds}ms",
					InfoLoggerMessageDuration.Short);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}

			IoC.Resolve<SimulationResultsProvider>().Value =
				new SimulationResultsBias(admittanceMatrix.GetNodes(), admittanceMatrix.ActiveComponentsCurrents);

			watch.Reset();

			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(simulationType));
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