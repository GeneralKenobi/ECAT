namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes providing results from simulation
	/// </summary>
	[NecessaryService]
	public interface ISimulationResultsProvider
	{
		#region Properties

		/// <summary>
		/// Results from the last simulation, non-null
		/// </summary>
		ISimulationResults Value { get; }

		#endregion
	}
}