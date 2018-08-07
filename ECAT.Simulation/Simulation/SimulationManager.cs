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
	public class SimulationManager : ISimulationManager
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion		

		#region Public methods

		/// <summary>
		/// Performs a single DC sweep for the given schematic
		/// </summary>
		/// <param name="schematic"></param>
		public void DCBias(ISchematic schematic)
		{
			// Create a stopwatch to measure the duration of procedures 
			Stopwatch watch = new Stopwatch();

			// Start it for admittance matrix creation
			watch.Start();

			// Create an admittance matrix
			var admittanceMatrix = AdmittanceMatrix.Construct(schematic);

			// Log the success and duration
			IoC.Log($"Constructed DC Admittance Matrix in {watch.ElapsedMilliseconds}ms", InfoLoggerMessageDuration.Short);
						
			watch.Restart();

			try
			{
				admittanceMatrix.ActivateCurrentSources();
				admittanceMatrix.ActivateDCVoltageSources();
				// Solve it (for now try-catch for debugging)
				admittanceMatrix.Solve();

				IoC.Log($"Calcualted the result in {watch.ElapsedMilliseconds}ms", InfoLoggerMessageDuration.Short);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}

			watch.Reset();
		}

		#endregion
	}
}