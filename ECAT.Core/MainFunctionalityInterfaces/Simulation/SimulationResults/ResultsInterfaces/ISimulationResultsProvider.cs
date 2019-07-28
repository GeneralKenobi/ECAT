using System.Collections.Generic;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes providing results from simulation
	/// </summary>
	[NecessaryService]
	public interface ISimulationResultsProvider: INotifyPropertyChanged
	{
		#region Properties

		/// <summary>
		/// Results from the last simulation, non-null
		/// </summary>
		ISimulationResults Value { get; }

		/// <summary>
		/// Measurements made by voltmeters
		/// </summary>
		IEnumerable<IVoltmeterMeasurement> DeclaredVoltmeterMeasurements { get; }

		#endregion
	}
}