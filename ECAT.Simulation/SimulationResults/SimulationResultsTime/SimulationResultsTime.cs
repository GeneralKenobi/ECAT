using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Manages results of time domain simulations
	/// </summary>
	public partial class SimulationResultsTime : ISimulationResults
	{
		#region Properties

		/// <summary>
		/// Contains information about power, guaranteed to be not null
		/// </summary>
		public IVoltageDB Voltage { get; }

		/// <summary>
		/// Contains information about power, guaranteed to be not null
		/// </summary>
		public ICurrentDB Current { get; }

		/// <summary>
		/// Contains information about power, guaranteed to be not null
		/// </summary>
		public IPowerDB Power { get; }

		#endregion
	}
}