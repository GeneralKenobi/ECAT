using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Implementation of the component declaration interface. Stores important information about a single component type
	/// </summary>
	public class ComponentDeclaration : IComponentDeclaration
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComponentDeclaration(ComponentIDEnumeration id, string displayName, int numberOfTerminals, ComponentType componentType)
		{
			ID = id;
			DisplayName = displayName;
			NumberOfTerminals = numberOfTerminals;
			ComponentType = componentType;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Unique number assigned to the specific component type
		/// </summary>
		public ComponentIDEnumeration ID { get; }

		/// <summary>
		/// User-friendly name that may be displayed on the screen
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Number of external terminals of the component
		/// </summary>
		public int NumberOfTerminals { get; }

		/// <summary>
		/// Type of the component
		/// </summary>
		public ComponentType ComponentType { get; }

		#endregion
	}
}