namespace ECAT.Core
{
	/// <summary>
	/// Interface for operational amplifiers. <see cref="IThreeTerminal.TerminalA"/> is the non-inverting input,
	/// <see cref="IThreeTerminal.TerminalB"/> is the inverting input and <see cref="IThreeTerminal.TerminalC"/> is the output.
	/// </summary>
	public interface IOpAmp : IThreeTerminal
	{
		#region Properties

		/// <summary>
		/// Positive supply voltage - output cannot be greater than this value
		/// </summary>
		double PositiveSupplyVoltage { get; set; }

		/// <summary>
		/// Negative supply voltage - output cannot be smaller than this value
		/// </summary>
		double NegativeSupplyVoltage { get; set; }

		/// <summary>
		/// Open loop gain - voltage gain defined as output voltage divided by differential voltage (U+ - U-)
		/// </summary>
		double OpenLoopGain { get; set; }

		#endregion
	}
}