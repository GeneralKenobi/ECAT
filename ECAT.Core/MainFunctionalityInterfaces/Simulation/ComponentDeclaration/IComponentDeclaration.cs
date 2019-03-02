namespace ECAT.Core
{
	/// <summary>
	/// Interface for component declarations - an identifying class for components that holds all important information about it
	/// </summary>
	public interface IComponentDeclaration
    {
		#region Properties

		/// <summary>
		/// Unique number assigned to the specific component type
		/// </summary>
		ComponentIDEnumeration ID { get; }

		/// <summary>
		/// User-friendly name that may be displayed on the screen
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Number of external terminals of the component
		/// </summary>
		int NumberOfTerminals { get; }

		/// <summary>
		/// Type of the component
		/// </summary>
		ComponentType ComponentType { get; }

		/// <summary>
		/// Category in which the component should be presented (e.g. in component adding menu)
		/// </summary>
		ComponentCategory Category { get; }

		#endregion
	}
}