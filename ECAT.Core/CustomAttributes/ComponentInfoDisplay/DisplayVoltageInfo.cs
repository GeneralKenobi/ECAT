using System;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to register an <see cref="IBaseComponent"/>'s voltage drop information between two terminals.
	/// Using it on classes that don't implement <see cref="IBaseComponent"/> will not do anything.
	/// <see cref="TerminalA"/> and <see cref="TerminalB"/> are names of public properties of type <see cref="ITerminal"/> and both
	/// have to obtainable from the target type (otherwise exception will be thrown during initalization).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class DisplayVoltageInfo : DisplayInfo
	{
		#region Constructors

		/// <summary>
		/// Default constructor for two-terminal voltage drop
		/// </summary>
		/// <param name="header">Header displayed above the info section</param>
		/// <param name="terminalA">Name of the property of the first (reference) terminal on target type. Can't be null or whitespace</param>
		/// <param name="terminalB">Name of the property of the second terminal on target type. Can't be null or whitespace</param>
		/// <param name="sectionIndex">Final position of the section, nonnegative, default value is <see cref="int.MaxValue"/></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public DisplayVoltageInfo(string terminalA, string terminalB, string header = "Voltage drop",
			int sectionIndex = int.MaxValue - 2) : base(sectionIndex, header)
		{
			TerminalA = string.IsNullOrWhiteSpace(terminalA) ? throw new ArgumentNullException(nameof(terminalA)) : terminalA;
			TerminalB = string.IsNullOrWhiteSpace(terminalB) ? throw new ArgumentNullException(nameof(terminalB)) : terminalB;
		}

		/// <summary>
		/// Default constructor for voltage drop between <paramref name="terminal"/> and ground
		/// </summary>
		/// <param name="header">Header displayed above the info section</param>
		/// <param name="terminalA">Name of the property of the first (reference) terminal on target type. Can't be null or whitespace</param>
		/// <param name="terminalB">Name of the property of the second terminal on target type. Can't be null or whitespace</param>
		/// <param name="sectionIndex">Final position of the section, nonnegative, default value is <see cref="int.MaxValue"/></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public DisplayVoltageInfo(string terminal, string header = "Voltage drop", int sectionIndex = int.MaxValue - 2)
			: base(sectionIndex, header)
		{
			TerminalB = string.IsNullOrWhiteSpace(terminal) ? throw new ArgumentNullException(nameof(terminal)) : terminal;
			TerminalA = string.Empty;
		}

		#endregion

		#region Public properties		

		/// <summary>
		/// First (reference) terminal taken for the voltage drop query (name of the property) or <see cref="string.Empty"/> for
		/// voltage drops taken from ground.
		/// </summary>
		public string TerminalA { get; }

		/// <summary>
		/// Second terminal taken for the voltage drop query (name of the property)
		/// </summary>
		public string TerminalB { get; }

		#endregion
	}
}