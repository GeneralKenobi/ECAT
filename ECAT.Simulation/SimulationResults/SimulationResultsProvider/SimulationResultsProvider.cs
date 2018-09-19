using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Provides results from the latest simulation
	/// </summary>
	// Register as self so that SimulationManager can use the public setter for Value
	[RegisterAsInstance(typeof(SimulationResultsProvider), typeof(ISimulationResultsProvider))]
	public class SimulationResultsProvider : ISimulationResultsProvider
	{
		#region Public properties

		/// <summary>
		/// Results from the last simulation
		/// </summary>
		public ISimulationResults Value { get; set; }

		#endregion
	}
}