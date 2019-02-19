namespace ECAT.Core
{
	/// <summary>
	/// Interface for active components that produce current which is calculated during the simulation rather than after simulation using
	/// component's parameters and voltage drop across the component or taken directly from component's parameters (<see cref="ICurrentSource"/>).
	/// </summary>
	public interface IActiveComponent : ISource
	{
		#region Properties

		/// <summary>
		/// Index used to query <see cref="ISimulationResults"/> for produced current
		/// </summary>
		int ActiveComponentIndex { get; set; }

		#endregion
	}
}