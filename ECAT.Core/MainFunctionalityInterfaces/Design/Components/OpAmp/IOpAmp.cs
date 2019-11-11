namespace ECAT.Core
{
	/// <summary>
	/// Interface for operational amplifiers. <see cref="IThreeTerminal.TerminalA"/> is the non-inverting input,
	/// <see cref="IThreeTerminal.TerminalB"/> is the inverting input and <see cref="IThreeTerminal.TerminalC"/> is the output.
	/// </summary>
	public interface IOpAmp : IThreeTerminal, IActiveComponent
	{
		#region Properties

		/// <summary>
		/// Open loop gain - voltage gain defined as output voltage divided by differential voltage (U+ - U-)
		/// </summary>
		double OpenLoopGain { get; set; }

		/// <summary>
		/// Description of this <see cref="IOpAmp"/>
		/// </summary>
		IOpAmpDescription Description { get; }

		#endregion
	}
}