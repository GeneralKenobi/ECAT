namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that manages and analyses results computed by <see cref="ISimulationManager"/>. If a value could not be
	/// calculated, it will be assigned <see cref="double.NaN"/>
	/// </summary>
	public interface ISimulationResults
    {
		#region Properties

		/// <summary>
		/// Contains information about power
		/// </summary>
		IVoltageDB Voltage { get; }

		/// <summary>
		/// Contains information about power
		/// </summary>
		ICurrentDB Current { get; }

		/// <summary>
		/// Contains information about power
		/// </summary>
		IPowerDB Power { get; }

		#endregion
	}
}