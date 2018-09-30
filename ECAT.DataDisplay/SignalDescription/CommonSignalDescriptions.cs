using ECAT.Core;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Contains descriptions of common signals
	/// </summary>
	[RegisterAsInstance(typeof(ICommonSignalDescriptions))]
	internal class CommonSignalDescriptions : ICommonSignalDescriptions
    {
		#region Public properties

		/// <summary>
		/// Description of a voltage signal
		/// </summary>
		public ISignalDescription Voltage { get; } = new SignalDescription("volt", "Volt", "V", "voltage", "Voltage");

		/// <summary>
		/// Description of a current signal
		/// </summary>
		public ISignalDescription Current { get; } = new SignalDescription("ampere", "Ampere", "A", "current", "Current");

		/// <summary>
		/// Description of a power signal
		/// </summary>
		public ISignalDescription Power { get; } = new SignalDescription("watt", "Watt", "W", "power", "Power");

		#endregion
	}
}