using System;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a module responsible for simulation and circuit operation
	/// </summary>
	public interface ISimulationManager : INotifyPropertyChanged
    {
		#region Events

		/// <summary>
		/// Event fired whenever simulation completes
		/// </summary>
		EventHandler<SimulationCompletedEventArgs> SimulationCompleted { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Constructs an appropriate admittance matrix for the given schematic and preforms a bias simulation
		/// </summary>
		/// <param name="schematic"></param>
		void Bias(ISchematic schematic, SimulationType simulationType);

		#endregion
	}
}