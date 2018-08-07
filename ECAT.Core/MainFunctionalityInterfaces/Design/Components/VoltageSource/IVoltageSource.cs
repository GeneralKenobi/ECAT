using CSharpEnhanced.CoreClasses;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for voltage sources (<see cref="ITwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="ITwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public interface IVoltageSource : ITwoTerminal
    {
		#region Properties

		/// <summary>
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		double ProducedDCVoltage { get; set; }

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		RefWrapperPropertyChanged<Complex> ProducedCurrent { get; set; }

		#endregion
	}
}