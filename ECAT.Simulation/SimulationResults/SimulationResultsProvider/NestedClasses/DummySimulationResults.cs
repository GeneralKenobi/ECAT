using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsProvider
	{
		/// <summary>
		/// Dummy simulation results with databases that always return null. To be used when <see cref="SimulationManager"/> did not
		/// provide a specific <see cref="ISimulationResults"/> (eg. because simulation was not yet run)
		/// </summary>
		private class DummySimulationResults : ISimulationResults
		{
			#region Public properties

			/// <summary>
			/// Dummy voltage database that always returns null
			/// </summary>
			public IVoltageDB Voltage { get; } = new DummyVoltageDB();

			/// <summary>
			/// Dummy current database that always returns null
			/// </summary>
			public ICurrentDB Current { get; } = new DummyCurrentDB();

			/// <summary>
			/// Dummy power database that always returns null
			/// </summary>
			public IPowerDB Power { get; } = new DummyPowerDB();

			#endregion
		}
	}
}