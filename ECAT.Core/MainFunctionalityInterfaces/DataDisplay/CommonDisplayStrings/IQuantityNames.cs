namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class providing unified display names of common quantities, eg. voltage.
	/// Its purpose is to unify common strings related to quantities throught application.
	/// </summary>
	public interface IQuantityNames
    {
		#region Properties

		/// <summary>
		/// Display name for voltage
		/// </summary>
		string Voltage { get; }

		/// <summary>
		/// Display name for voltage with first letter capitalized
		/// </summary>
		string VoltageCap { get; }

		/// <summary>
		/// Display name for current
		/// </summary>
		string Current { get; }

		/// <summary>
		/// Display name for current with first letter capitalized
		/// </summary>
		string CurrentCap { get; }

		/// <summary>
		/// Display name for power
		/// </summary>
		string Power { get; }

		/// <summary>
		/// Display name for power with first letter capitalized
		/// </summary>
		string PowerCap { get; }

		/// <summary>
		/// Display name for frequency
		/// </summary>
		string Frequency { get; }

		/// <summary>
		/// Display name for frequency with first letter capitalized
		/// </summary>
		string FrequencyCap { get; }

		#endregion
	}
}