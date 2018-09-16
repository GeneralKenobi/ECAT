using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Provides results from the latest simulation
	/// </summary>
	[RegisterAsInstance(typeof(ISimulationResultsProvider))]
	public class SimulationResultsProvider : ISimulationResultsProvider
	{
		#region Public methods

		/// <summary>
		/// Results from the last simulation
		/// </summary>
		public ISimulationResults Value { get; set; }

		#endregion
	}
}