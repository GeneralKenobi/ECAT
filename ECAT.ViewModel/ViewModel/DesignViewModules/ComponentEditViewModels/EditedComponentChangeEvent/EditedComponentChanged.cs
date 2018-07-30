namespace ECAT.ViewModel
{
	/// <summary>
	/// Enum denoting type of change of the edited component
	/// </summary>
	public enum EditedComponentChanged
	{
		/// <summary>
		/// Old value was null, new is a component
		/// </summary>
		NullToComponent = 0,

		/// <summary>
		/// Old value was a component, new is null
		/// </summary>
		ComponentToNull = 1,

		/// <summary>
		/// Old value was a component, new is a different part
		/// </summary>
		ComponentToComponent = 2,

		/// <summary>
		/// Old value is the same as new (but it's not null)
		/// </summary>
		NoChange = 3,
	}
}