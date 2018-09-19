using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Provides results from the latest simulation
	/// </summary>
	// Register as self so that SimulationManager can use the public setter for Value
	[RegisterAsInstance(typeof(SimulationResultsProvider), typeof(ISimulationResultsProvider))]
	public partial class SimulationResultsProvider : ISimulationResultsProvider
	{
		#region Private members

		/// <summary>
		/// Backing store for <see cref="Value"/>
		/// </summary>
		private ISimulationResults mValue;

		#endregion

		#region Private properties
		
		/// <summary>
		/// Dummy results that should be returned by <see cref="Value"/> if <see cref="mValue"/> is null
		/// </summary>
		private ISimulationResults _DummySimulationResults { get; } = new DummySimulationResults();

		#endregion

		#region Public properties

		/// <summary>
		/// Results from the last simulation
		/// </summary>
		public ISimulationResults Value
		{
			get => mValue ?? _DummySimulationResults;
			set => mValue = value;
		}

		#endregion
	}
}