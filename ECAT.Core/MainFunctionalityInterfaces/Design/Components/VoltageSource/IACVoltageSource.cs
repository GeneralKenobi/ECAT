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
		/// Frequency of the produced voltage
		/// </summary>
		Variable FrequencyVar { get; }

		/// <summary>
		/// Accessor to the frequency of produced voltage
		/// </summary>
		double Frequency { get; set; }

		#endregion
	}
}