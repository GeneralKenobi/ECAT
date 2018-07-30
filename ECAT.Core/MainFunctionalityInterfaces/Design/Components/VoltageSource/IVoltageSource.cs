using CSharpEnhanced.CoreClasses;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for voltage sources
	/// </summary>
	public interface IVoltageSource : ITwoTerminal
    {
		#region Properties

		/// <summary>
		/// Current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		double ProducedVoltage { get; set; }

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		RefWrapperPropertyChanged<double> ProducedCurrent { get; set; }

		#endregion
	}
}