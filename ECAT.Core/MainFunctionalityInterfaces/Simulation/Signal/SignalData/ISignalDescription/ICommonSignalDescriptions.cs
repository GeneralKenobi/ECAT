namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing <see cref="ISignalDescription"/>s for common signals
	/// </summary>
	public interface ICommonSignalDescriptions
    {
		#region Public properties

		/// <summary>
		/// Description of a voltage signal
		/// </summary>
		ISignalDescription Voltage { get; }

		/// <summary>
		/// Description of a current signal
		/// </summary>
		ISignalDescription Current { get; }

		/// <summary>
		/// Description of a power signal
		/// </summary>
		ISignalDescription Power { get; }

		#endregion
	}
}