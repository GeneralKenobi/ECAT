namespace ECAT.Core
{
	/// <summary>
	/// Interface for all sources
	/// </summary>
	public interface ISource
	{
		#region Properties

		/// <summary>
		/// Description of this <see cref="ISource"/>
		/// </summary>
		IActiveComponentDescription Description { get; }

		#endregion
	}
}