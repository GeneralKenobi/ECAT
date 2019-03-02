namespace ECAT.Core
{
	/// <summary>
	/// Interface for active components that produce current which is calculated during the simulation rather than after simulation using
	/// component's parameters and voltage drop across the component or taken directly from component's parameters (<see cref="ICurrentSource"/>).
	/// </summary>
	public interface IActiveComponent
	{
		#region Properties

		/// <summary>
		/// Index assigned to this <see cref="IActiveComponent"/>
		/// TODO: Make it so that only the designated classes can assign it, this is temporary
		/// </summary>
		int Index { get; set; }

		#endregion
	}
}