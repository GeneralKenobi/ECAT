using System;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a module responsible for simulation and circuit operation
	/// </summary>
	[NecessaryService]
	public interface ISimulationManager : INotifyPropertyChanged
    {
		#region Events

		/// <summary>
		/// Event fired whenever simulation completes
		/// </summary>
		event EventHandler<SimulationCompletedEventArgs> SimulationCompleted;

		#endregion

		#region Methods

		/// <summary>
		/// Performs a DC bias simulation of the circuit.
		/// </summary>
		/// <param name="schematic"></param>
		void DCBias(ISchematic schematic);

		/// <summary>
		/// Performs a full cycle AC simulation
		/// </summary>
		/// <param name="schematic"></param>
		void ACFullCycle(ISchematic schematic);

		/// <summary>
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		void ACDCFullCycle(ISchematic schematic);

		#endregion
	}
}