namespace ECAT.Core
{
	/// <summary>
	/// Type of power dissipation
	/// </summary>
	public enum PowerType
    {
		/// <summary>
		/// Power is equal to 0 - it is neither dissipated nor supplied
		/// </summary>
		None = 0,

		/// <summary>
		/// Power is dissipated - lost on the element
		/// </summary>
		Dissipated = 1,

		/// <summary>
		/// Power is supplied by the element
		/// </summary>
		Supplied = 2,
    }
}