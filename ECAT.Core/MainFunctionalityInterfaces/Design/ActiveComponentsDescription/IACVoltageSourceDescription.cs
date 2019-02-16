namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes used to describe <see cref="IACVoltageSource"/>s
	/// </summary>
	public interface IACVoltageSourceDescription : IActiveComponentDescription
	{
		#region Properties

		/// <summary>
		/// Frequency of the described <see cref="IACVoltageSource"/>
		/// </summary>
		double Frequency { get; }

		/// <summary>
		/// Peak voltage produced by the described <see cref="IACVoltageSource"/>
		/// </summary>
		double PeakProducedVoltage { get; }

		#endregion
	}
}