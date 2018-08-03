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
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		double ProducedDCVoltage { get; set; }

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		RefWrapperPropertyChanged<double> ProducedCurrent { get; set; }

		#endregion
	}
}