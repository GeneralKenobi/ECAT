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
		/// Default constructor
		/// </summary>
		/// <param name="header">Header displayed above the info section</param>
		/// <param name="terminalA">Name of the property of the first (reference) terminal on target type</param>
		/// <param name="terminalB">Name of the property of the second terminal on target type</param>
		/// <param name="sectionIndex">Final position of the section, nonnegative, default value is <see cref="int.MaxValue"/></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public DisplayVoltageInfo(string terminalA, string terminalB, string header = "Voltage drop", int sectionIndex = int.MaxValue) :
			base(sectionIndex, header)
		{
			TerminalA = terminalA ?? throw new ArgumentNullException(nameof(terminalA));
			TerminalB = terminalB ?? throw new ArgumentNullException(nameof(terminalB));
		}

		#endregion

		#region Public properties		

		/// <summary>
		/// First (reference) terminal taken for the voltage drop query (name of the property)
		/// </summary>
		public string TerminalA { get; }

		/// <summary>
		/// Second terminal taken for the voltage drop query (name of the property)
		/// </summary>
		public string TerminalB { get; }

		#endregion
	}
}