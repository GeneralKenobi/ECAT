namespace ECAT.Core.GlobalEnums
{
	/// <summary>
	/// Type of power dissipation
	/// </summary>
	public enum PowerType
    {
		/// <summary>
		/// Power is dissipated - lost on the element
		/// </summary>
		Dissipated = 0,

		/// <summary>
		/// Power is supplied by the element
		/// </summary>
		Supplied = 1,
    }
}