namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes used to describe <see cref="IActiveComponent"/>s
	/// </summary>
	public interface IActiveComponentDescription
	{
		#region Properties

		/// <summary>
		/// Unique label assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		string Label { get; }

		/// <summary>
		/// Active component index assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		int Index { get; }

		#endregion
	}
}