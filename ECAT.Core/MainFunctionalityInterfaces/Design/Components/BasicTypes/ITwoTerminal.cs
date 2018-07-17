namespace ECAT.Core
{
	/// <summary>
	/// Interface for all components that have two terminals
	/// </summary>
	public interface ITwoTerminal : IBaseComponent
    {
		#region Properties

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		IPlanePosition TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		IPlanePosition TerminalB { get; }

		#endregion
	}
}