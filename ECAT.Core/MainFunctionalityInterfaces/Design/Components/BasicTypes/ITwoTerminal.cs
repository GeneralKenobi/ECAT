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
		/// True if the standard voltage drop direciton (Vb - Va) was inverted
		/// </summary>
		bool InvertedVoltageCurrentDirections { get; }

		/// <summary>
		/// Voltage drop across the part. The direction is determined by <see cref="InvertedVoltageCurrentDirections"/>
		/// </summary>
		Complex VoltageDrop { get; }

		/// <summary>
		/// The maximum voltage drop that may be observed across the component
		/// </summary>
		Complex MaximumVoltageDrop { get; }

		/// <summary>
		/// The minimum voltage drop that may be observed across the component
		/// </summary>
		Complex MinimumVoltageDrop { get; }

		/// <summary>
		/// The RMS value of voltage across the component
		/// </summary>
		Complex RMSVoltageDrop { get; }

		/// <summary>
		/// Current through the component, by convention (although not always as, for example, voltage sources will align current and
		/// voltage drop in the same direction. All things considered <see cref="VoltageDrop"/> and
		/// <see cref="InvertedVoltageCurrentDirections"/> are guaranteed to be mutually correct) it flows in direction opposite to voltage
		/// drop (so for <see cref="InvertedVoltageCurrentDirections"/> equal to false the current is given from <see cref="TerminalB"/> to
		/// <see cref="TerminalA"/>)
		Complex Current { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// <paramref name="frequency">Frequency of the considered signal, cannot be smaller than 0, 0 indicates DC signal</paramref>
		/// </summary>
		Complex GetAdmittance(double frequency);

		#endregion
	}
}