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
		/// <param name="resistor"></param>
		/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
		/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
		/// <returns></returns>
		ISignalInformation Get(IResistor resistor, bool voltageBA);

		/// <summary>
		/// Gets information about current flowing through an <see cref="ICapacitor"/> or null if unsuccessful
		/// </summary>		
		/// <param name="capacitor"></param>
		/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
		/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
		/// <returns></returns>
		ISignalInformation Get(ICapacitor capacitor, bool voltageBA);

		/// <summary>
		/// Returns current produced by some <see cref="IActiveComponent"/>. If simulation was not yet performed or the current can't be
		/// found returns null
		/// </summary>
		/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
		/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
		/// by convention for the specific element (obtained during simulation)</param>
		/// <returns></returns>
		ISignalInformation Get(int activeComponentIndex, bool reverseDirection);

		#endregion
	}
}