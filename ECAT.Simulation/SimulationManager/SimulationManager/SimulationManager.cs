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
		private SimulationResults _Results { get; } = new SimulationResults();

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

			_Results.LoadNewData(admittanceMatrix.Nodes, admittanceMatrix.ActiveComponentsCurrents);

			watch.Reset();

			SimulationCompleted?.Invoke(this, new SimulationCompletedEventArgs(simulationType));
		}

		/// <summary>
		/// Returns an enumeration of types to register in IoC. Item1 is the interface to register as, Item2 is the object to register
		/// as an instance.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Tuple<Type, object>> GetInstancesToRegister()
		{
			yield return new Tuple<Type, object>(typeof(IDefaultValues), new DefaultValues());
			yield return new Tuple<Type, object>(typeof(ISimulationResults), _Results);
		}

		/// <summary>
		/// Returns an enumeration of types to register in IoC. Item1 is the interface to register as, Item2 is the type to register
		/// as implementation of the interface
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Tuple<Type, Type>> GetTypesToRegister() => Enumerable.Empty<Tuple<Type, Type>>();

		#endregion

		#region Public static properties

		/// <summary>
		/// The index assumed for ground nodes
		/// </summary>
		public static int GroundNodeIndex { get; } = 0;

		#endregion
	}
}