using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a module responsible for simulation and circuit operation
	/// </summary>
	public interface ISimulationManager : INotifyPropertyChanged
    {
		#region Properties

		/// <summary>
		/// Collection of names of all components that are implemented and usable
		/// </summary>
		ReadOnlyObservableCollection<IComponentDeclaration> ImplementedComponents { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Constructs a DC admittance matrix for the given schematic
		/// </summary>
		/// <param name="schematic"></param>
		void ConstructDCAdmittanceMatrix(ISchematic schematic);

		#endregion
	}
}