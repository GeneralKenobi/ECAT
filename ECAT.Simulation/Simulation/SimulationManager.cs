using ECAT.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of the simulation module
	/// </summary>
	public partial class SimulationManager : ISimulationManager
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired whenever simulation completes
		/// </summary>
		public EventHandler<SimulationCompletedEventArgs> SimulationCompleted { get; set; }

		#endregion

		#region Private properties

		/// <summary>
		/// The result manager for the app's whole lifetime
		/// </summary>
		private ISimulationResultManager _ResultManager { get; } = IoC.Resolve<ISimulationResultManager>();

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

			watch.Reset();			

			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(simulationType));
		}

		#endregion
	}
}