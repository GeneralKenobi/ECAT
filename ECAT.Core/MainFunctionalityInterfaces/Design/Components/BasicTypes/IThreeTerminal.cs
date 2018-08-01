using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for all components that have three terminals
	/// </summary>
	public interface IThreeTerminal : IBaseComponent
	{
		#region Properties

		/// <summary>
		/// One of the terminals in this three-terminal
		/// </summary>
		ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this three-terminal
		/// </summary>
		ITerminal TerminalB { get; }

		/// <summary>
		/// One of the terminals in this three-terminal
		/// </summary>
		ITerminal TerminalC { get; }

		/// <summary>
		/// The admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// </summary>
		Complex AdmittanceAB { get; set; }

		/// <summary>
		/// The admittance between <see cref="TerminalA"/> and <see cref="TerminalC"/>
		/// </summary>
		Complex AdmittanceAC { get; set; }

		/// <summary>
		/// The admittance between <see cref="TerminalB"/> and <see cref="TerminalC"/>
		/// </summary>
		Complex AdmittanceBC { get; set; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalB"/> and <see cref="TerminalA"/>
		/// </summary>
		double VoltageBA { get; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalC"/> and <see cref="TerminalA"/>
		/// </summary>
		double VoltageCA { get; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalC"/> and <see cref="TerminalB"/>
		/// </summary>
		double VoltageCB { get; }

		#endregion
	}
}