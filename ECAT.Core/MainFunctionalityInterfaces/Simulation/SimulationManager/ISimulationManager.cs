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
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		void ACDCFullCycle(ISchematic schematic);

		/// <summary>
		/// Performs a full ACDC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		void FrequencySweep(ISchematic schematic);

		#endregion
	}
}