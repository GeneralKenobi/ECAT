namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes providing description of an <see cref="ISignalInformation"/> (name, unit, etc).
	/// </summary>
	public interface ISignalDescription
    {
		#region Properties

		/// <summary>
		/// Full name of the unit of this signal (eg. "ampere")
		/// </summary>
		string Unit { get; }

		/// <summary>
		/// Full name of the unit of this signal with first letter capitilized (eg. "Ampere")
		/// </summary>
		string UnitCap { get; }

		/// <summary>
		/// Short name of the unit of this signal (eg. "A")
		/// </summary>
		string UnitShort { get; }

		/// <summary>
		/// Name of the signal (eg. "current")
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Name of the signal with first letter capitilized (eg. "Current")
		/// </summary>
		string NameCap { get; }

		#endregion
	}
}