namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes holding information about currents flowing through components
	/// </summary>
	public interface ICurrentDB
    {
		#region Methods

		/// <summary>
		/// Gets information about current flowing through an <see cref="IResistor"/> or null if unsuccessful
		/// </summary>
		/// <param name="voltageDrop"></param>
		/// <param name="resistor"></param>
		/// <returns></returns>
		ISignalInformation GetCurrent(IResistor resistor, bool reverseDirection);

		/// <summary>
		/// Gets information about current flowing through an <see cref="ICapacitor"/> or null if unsuccessful
		/// </summary>
		/// <param name="voltageDrop"></param>
		/// <param name="capacitor"></param>
		/// <returns></returns>
		ISignalInformation GetCurrent(ICapacitor capacitor, bool reverseDirection);

		/// <summary>
		/// Returns current produced by some <see cref="IActiveComponent"/>. If simulation was not yet performed or the current can't be
		/// found returns null
		/// </summary>
		/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
		/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
		/// by convention for the specific element</param>
		/// <returns></returns>
		ISignalInformation GetCurrent(int activeComponentIndex, bool reverseDirection);

		#endregion
	}
}