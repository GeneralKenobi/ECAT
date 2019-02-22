namespace ECAT.Core
{
	/// <summary>
	/// Basic interface that can be used as description of any <see cref="IBaseComponent"/>
	/// </summary>
	public interface IComponentDescription
	{
		#region Properties

		/// <summary>
		/// Unique label assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		IIDLabel Label { get; }

		#endregion
	}
}