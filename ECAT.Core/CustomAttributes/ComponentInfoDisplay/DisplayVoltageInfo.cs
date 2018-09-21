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
	public class DisplayVoltageInfo : Attribute
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public DisplayVoltageInfo(string header, string terminalA, string terminalB)
		{
			Header = header ?? throw new ArgumentNullException(nameof(header));
			TerminalA = terminalA ?? throw new ArgumentNullException(nameof(terminalA));
			TerminalB = terminalB ?? throw new ArgumentNullException(nameof(terminalB));
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Header to display for that info section
		/// </summary>
		public string Header { get; }

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