namespace ECAT.Simulation
{
	/// <summary>
	/// Possible operation modes for an <see cref="ITransistor"/>
	/// </summary>
	public enum TransistorOperationMode
	{
		/// <summary>
		/// <see cref="Itransistor"/> is in active mode - channel current is can be controlled
		/// </summary>
		Active = 0,

		/// <summary>
		/// No current flows through the transistor
		/// </summary>
		Cutoff = 1,

		/// <summary>
		/// Maximum current flows throgh the transistor, it can no longer be controlled
		/// </summary>
		Saturation = 2,

		/// <summary>
		/// Transistor operates with its small signal model - this is preset by user
		/// </summary>
		SmallSignal = 3,
	}
}