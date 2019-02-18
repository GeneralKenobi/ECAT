namespace ECAT.Core
{
	/// <summary>
	/// Interface for alternating voltage sources
	/// </summary>
	public interface IACVoltageSource : ITwoTerminal, IActiveComponent
	{
		#region Properties

		/// <summary>
		/// Frequency of the AC voltage produced by this <see cref="IACVoltageSource"/>
		/// </summary>
		double Frequency { get; set; }

		/// <summary>
		/// The positive peak value of the produced voltage sine wave
		/// </summary>
		double PeakProducedVoltage { get; set; }

		#endregion
	}
}