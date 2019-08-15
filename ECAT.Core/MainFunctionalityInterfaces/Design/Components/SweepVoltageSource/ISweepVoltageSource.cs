namespace ECAT.Core
{
	/// <summary>
	/// Interface for sweep voltage sources used to perform parametric simulations (
	/// </summary>
	public interface ISweepVoltageSource: IACVoltageSource
	{
		#region Properties

		/// <summary>
		/// Frequency from which to start the sweep
		/// </summary>
		double StartFrequency { get; set; }

		/// <summary>
		/// Frequency at which to end the sweep
		/// </summary>
		double EndFrequency { get; set; }

		#endregion
	}
}