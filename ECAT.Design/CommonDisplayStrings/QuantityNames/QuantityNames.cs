using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IQuantityNames"/>
	/// </summary>
	public class QuantityNames : IQuantityNames
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		private QuantityNames() { }

		#endregion

		#region Public properties

		/// <summary>
		/// Display name for voltage
		/// </summary>
		public string Voltage { get; } = "voltage";

		/// <summary>
		/// Display name for voltage with first letter capitalized
		/// </summary>
		public string VoltageCap { get; } = "Voltage";

		/// <summary>
		/// Display name for current
		/// </summary>
		public string Current { get; } = "current";

		/// <summary>
		/// Display name for current with first letter capitalized
		/// </summary>
		public string CurrentCap { get; } = "Current";

		/// <summary>
		/// Display name for power
		/// </summary>
		public string Power { get; } = "power";

		/// <summary>
		/// Display name for power with first letter capitalized
		/// </summary>
		public string PowerCap { get; } = "Power";

		/// <summary>
		/// Display name for frequency
		/// </summary>
		public string Frequency { get; } = "frequency";

		/// <summary>
		/// Display name for frequency with first letter capitalized
		/// </summary>
		public string FrequencyCap { get; } = "Frequency";

		#endregion

		#region Singleton

		/// <summary>
		/// Singleton instance of this class
		/// </summary>
		public static QuantityNames Singleton = new QuantityNames();

		#endregion
	}
}