namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes providing results from simulation
	/// </summary>
	public interface ISimulationResultsProvider
	{
		#region Properties

		/// <summary>
		/// Results from the last simulation
		/// </summary>
		ISimulationResults Value { get; }

		#endregion
	}
}