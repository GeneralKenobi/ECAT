namespace ECAT.Core
{
	/// <summary>
	/// Interface for voltage sources (<see cref="ITwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="ITwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public interface IVoltageSource : ITwoTerminal, IActiveComponent
    {
		#region Properties

		/// <summary>
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		double ProducedDCVoltage { get; set; }

		#endregion
	}
}