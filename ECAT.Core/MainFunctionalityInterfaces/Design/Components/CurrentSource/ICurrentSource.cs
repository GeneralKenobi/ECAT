namespace ECAT.Core
{
	/// <summary>
	/// Interface for current sources (<see cref="ITwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="ITwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public interface ICurrentSource : ITwoTerminal, ISource
    {

	}
}