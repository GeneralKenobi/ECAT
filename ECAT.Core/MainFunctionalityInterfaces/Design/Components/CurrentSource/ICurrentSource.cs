namespace ECAT.Core
{
	/// <summary>
	/// Interface for current sources (<see cref="ITwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="ITwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public interface ICurrentSource : ITwoTerminal, IActiveComponent
    {
		#region Properties

		/// <summary>
		/// Current produced by this <see cref="ICurrentSource"/>
		/// </summary>
		double ProducedCurrent { get; set; }

		#endregion
	}
}