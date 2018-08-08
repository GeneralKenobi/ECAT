using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a module responsible for simulation and circuit operation
	/// </summary>
	public interface ISimulationManager : INotifyPropertyChanged
    {
		#region Methods

		/// <summary>
		/// Constructs a DC admittance matrix for the given schematic and preforms a single DC simulation
		/// </summary>
		/// <param name="schematic"></param>
		void DCBias(ISchematic schematic);

		/// <summary>
		/// Constructs an AC admittance matrix for the given schematic and performs a single AC simulation
		/// </summary>
		/// <param name="schematic"></param>
		void ACBias(ISchematic schematic);

		#endregion
	}
}