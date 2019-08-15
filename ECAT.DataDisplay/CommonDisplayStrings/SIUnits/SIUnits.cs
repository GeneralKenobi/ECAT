using ECAT.Core;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Standard implementation of <see cref="ISIUnits"/>, contains units of quantities
	/// </summary>
	[RegisterAsInstance(typeof(ISIUnits))]
	public class SIUnits : ISIUnits
	{
		#region Public properties

		/// <summary>
		/// Full name of unit of voltage
		/// </summary>
		public string Voltage { get; } = "Volt";

		/// <summary>
		/// Shortened name of voltage unit
		/// </summary>
		public string VoltageShort { get; } = "V";

		/// <summary>
		/// Full name of unit of current
		/// </summary>
		public string Current { get; } = "Ampere";

		/// <summary>
		/// Shortened name of current unit
		/// </summary>
		public string CurrentShort { get; } = "A";

		/// <summary>
		/// Full name of unit of gain
		/// </summary>
		public string Gain { get; } = "Decibels";

		/// <summary>
		/// Shortened name of gain unit
		/// </summary>
		public string GainShort { get; } = "dB";

		/// <summary>
		/// Full name of unit of power
		/// </summary>
		public string Power { get; } = "Watt";

		/// <summary>
		/// Shortened name of power unit
		/// </summary>
		public string PowerShort { get; } = "W";

		/// <summary>
		/// Full name of unit of frequency
		/// </summary>
		public string Frequency { get; } = "Hertz";

		/// <summary>
		/// Shortened name of frequency unit
		/// </summary>
		public string FrequencyShort { get; } = "Hz";

		/// <summary>
		/// Full name of time unit
		/// </summary>
		public string Time { get; } = "Second";

		/// <summary>
		/// Shortened name of time unit
		/// </summary>
		public string TimeShort { get; } = "s";

		/// <summary>
		/// Full name of degrees unit
		/// </summary>
		public string Phase { get; } = "Degrees";

		#endregion
	}
}