using CSharpEnhanced.Maths;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for alternating voltage sources
	/// </summary>
	public interface IACVoltageSource : IVoltageSource
    {
		#region Properties

		/// <summary>
		/// Frequency of the AC voltage produced by this <see cref="IACVoltageSource"/>
		/// </summary>
		double Frequency { get; set; }

		#endregion
	}
}