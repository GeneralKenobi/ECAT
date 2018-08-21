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

		#endregion

		#region Methods

		/// <summary>
		/// Admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// <paramref name="frequency">Frequency of the considered signal, cannot be smaller than 0, 0 indicates DC signal</paramref>
		/// </summary>
		Complex GetAdmittance(double frequency);

		/// <summary>
		/// Returns conductance between <see cref="TerminalA"/> and <see cref="TerminalB"/> for DC signals. Conductance is the real part
		/// of admittance. For DC all elements have admittance with only real component being non-zero so this method returns the same
		/// as <see cref="GetAdmittance(double)"/> for 0 frequency.
		/// </summary>
		/// <returns></returns>
		double GetConductance();

		#endregion
	}
}