using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="ISourceDescription"/> - this class describes <see cref="IActiveComponent"/>s.
	/// </summary>
	public class SourceDescription : ComponentDescription, ISourceDescription
	{
		#region Public Properties

		/// <summary>
		/// Frequency of the described <see cref="IActiveComponent"/>
		/// </summary>
		public double Frequency { get; set; }

		/// <summary>
		/// Type of the <see cref="IActiveComponent"/>
		/// </summary>
		public SourceType SourceType { get; set; }

		/// <summary>
		/// Frequency category to which this source belongs
		/// </summary>
		public FrequencyCategory FrequencyCategory => SourceType == SourceType.ACVoltageSource ? FrequencyCategory.AC : FrequencyCategory.DC;

		/// <summary>
		/// Output value produced by the source (voltage, current, etc.)
		/// </summary>
		public double OutputValue { get; set; }

		#endregion
	}
}