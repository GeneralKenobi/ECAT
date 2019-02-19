namespace ECAT.Core
{
	/// <summary>
	/// Interface for all sources
	/// </summary>
	public interface ISource
	{
		#region Properties

		/// <summary>
		/// Index used to query <see cref="ISimulationResults"/> for produced current
		/// </summary>
		int ActiveComponentIndex { get; set; }

		/// <summary>
		/// Description of this <see cref="ISource"/>
		/// </summary>
		IActiveComponentDescription Description { get; }

		#endregion
	}
}