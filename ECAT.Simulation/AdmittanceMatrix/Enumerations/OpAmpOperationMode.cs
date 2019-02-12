namespace ECAT.Simulation
{
	/// <summary>
	/// Possible operation modes for an <see cref="IOpAmp"/>
	/// </summary>
	public enum OpAmpOperationMode
	{
		/// <summary>
		/// <see cref="IOpAmp"/> is in active mode - it's an amplifier, output voltage is between supply voltages
		/// </summary>
		Active = 0,

		/// <summary>
		/// Output voltage is constant and equal to positive supply voltage
		/// </summary>
		PositiveSaturation = 1,

		/// <summary>
		/// Output voltage is constant and equal to negative supply voltage
		/// </summary>
		NegativeSaturation = 2,
	}
}