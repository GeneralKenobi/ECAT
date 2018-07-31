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
		/// Constructs a DC admittance matrix for the given schematic
		/// </summary>
		/// <param name="schematic"></param>
		void DCBias(ISchematic schematic);

		#endregion
	}
}