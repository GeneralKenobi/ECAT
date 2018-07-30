namespace ECAT.Core
{
	/// <summary>
	/// Defines types of components
	/// </summary>
	public enum ComponentType
    {
		/// <summary>
		/// Passive components cannot affect the flow of electrons (eg. resistors, capacitors)
		/// </summary>
		Passive = 0,

		/// <summary>
		/// Active components can affect the flow of electrons (eg. bipolar junction transistors)
		/// </summary>
		Active = 1,
    }
}