using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IComponentDescription"/>
	/// </summary>
	public class ComponentDescription : IComponentDescription
	{
		#region Public properties

		/// <summary>
		/// Unique label assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		public IIDLabel Label { get; set; }

		#endregion
	}
}