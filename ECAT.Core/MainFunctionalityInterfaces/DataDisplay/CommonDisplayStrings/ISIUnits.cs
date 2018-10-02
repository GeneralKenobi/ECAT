namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class containing information about untis for quantities. Its purpose is to unify units throught application.
	/// </summary>
	[NecessaryService]
	public interface ISIUnits
    {
		#region Properties

		/// <summary>
		/// Full name of unit of voltage
		/// </summary>
		string Voltage { get; }

		/// <summary>
		/// Shortened name of voltage unit
		/// </summary>
		string VoltageShort { get; }

		/// <summary>
		/// Full name of unit of current
		/// </summary>
		string Current { get; }

		/// <summary>
		/// Shortened name of current unit
		/// </summary>
		string CurrentShort { get; }

		/// <summary>
		/// Full name of unit of power
		/// </summary>
		string Power { get; }

		/// <summary>
		/// Shortened name of power unit
		/// </summary>
		string PowerShort { get; }

		/// <summary>
		/// Full name of unit of frequency
		/// </summary>
		string Frequency { get; }

		/// <summary>
		/// Shortened name of frequency unit
		/// </summary>
		string FrequencyShort { get; }

		#endregion
	}
}