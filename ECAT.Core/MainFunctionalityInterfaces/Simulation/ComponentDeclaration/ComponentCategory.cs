namespace ECAT.Core
{
	/// <summary>
	/// Categorizes components into groups which will be used to create component adding menu. Each group is presented separately.
	/// </summary>
	public enum ComponentCategory
	{
		/// <summary>
		/// Sources - e.g. voltage / current sources
		/// </summary>
		Source = 0,

		/// <summary>
		/// General impedances - resistors, capacitors, etc.
		/// </summary>
		Impedance = 1,

		/// <summary>
		/// Three terminal elements - operational amplifiers, transistors
		/// </summary>
		ThreeTerminal = 2,

		/// <summary>
		/// All components that don't fit any of the categories above
		/// </summary>
		Other = 3,
	}
}