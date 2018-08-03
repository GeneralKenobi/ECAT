using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for all components that have two terminals
	/// </summary>
	public interface ITwoTerminal : IBaseComponent
    {
		#region Properties

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		ITerminal TerminalB { get; }

		/// <summary>
		/// Admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// </summary>
		Complex Admittance { get; set; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalB"/> and <see cref="TerminalA"/> (VB - VA)
		/// </summary>
		double VoltageBA { get; }

		/// <summary>
		/// Current flowing from <see cref="TerminalA"/> to <see cref="TerminalB"/>
		/// </summary>
		double CurrentBA { get; }

		#endregion
	}
}