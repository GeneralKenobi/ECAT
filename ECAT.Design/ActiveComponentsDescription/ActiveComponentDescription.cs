using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IActiveComponentDescription"/> - this class describes <see cref="IActiveComponent"/>s.
	/// </summary>
	public class ActiveComponentDescription : IActiveComponentDescription
	{
		#region Public properties

		/// <summary>
		/// Unique label assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// Active component index assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		public int Index { get; set; }

		#endregion
	}
}