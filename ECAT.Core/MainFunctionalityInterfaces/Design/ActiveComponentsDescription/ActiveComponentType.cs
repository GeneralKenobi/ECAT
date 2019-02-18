namespace ECAT.Core
{
	/// <summary>
	/// Enumerates possible types of <see cref="IActiveComponent"/>s
	/// </summary>
	public enum ActiveComponentType
	{
		/// <summary>
		/// DC voltage source
		/// </summary>
		DCVoltageSource = 0,

		/// <summary>
		/// AC voltage source
		/// </summary>
		ACVoltageSource = 1,

		/// <summary>
		/// Operational Amplifier
		/// </summary>
		OpAmp = 2,
	}
}