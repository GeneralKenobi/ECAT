using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IACVoltageSourceDescription"/> - this class describes <see cref="IACVoltageSource"/>s.
	/// </summary>
	public class ACVoltageSourceDescription : ActiveComponentDescription, IACVoltageSourceDescription
	{
		#region Public properties

		/// <summary>
		/// Frequency of the described <see cref="IACVoltageSource"/>
		/// </summary>
		public double Frequency { get; set; }

		/// <summary>
		/// Peak voltage produced by the described <see cref="IACVoltageSource"/>
		/// </summary>
		public double PeakProducedVoltage { get; set; }

		#endregion
	}
}