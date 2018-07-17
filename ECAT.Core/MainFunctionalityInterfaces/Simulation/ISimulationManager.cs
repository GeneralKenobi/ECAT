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

		/// <summary>
		/// A maximum value for parameters in the circuit (admittance, voltage source voltage, etc)
		/// </summary>
		double MaximumParameterValue { get; }

		#endregion
	}
}