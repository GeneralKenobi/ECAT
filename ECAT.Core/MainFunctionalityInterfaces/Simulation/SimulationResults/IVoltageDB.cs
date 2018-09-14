namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes holding information about voltage drops calculated in simulation
	/// </summary>
	public interface IVoltageDB
	{
		#region Methods

		/// <summary>
		/// Gets a voltage drop of a node with respect to ground or returns null if unsuccessful
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <returns></returns>
		ISignalInformation GetVoltageDrop(int nodeIndex);

		/// <summary>
		/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns null
		/// if unsuccessful
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <returns></returns>
		ISignalInformation GetVoltageDrop(int nodeAIndex, int nodeBIndex);

		/// <summary>
		/// Gets information on voltage drop across a <see cref="ITwoTerminal"/> component
		/// </summary>
		/// <param name="component"></param>
		/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
		/// <see cref="ITwoTerminal.TerminalA"/>, if false from <see cref="ITwoTerminal.TerminalA"/> to
		/// <see cref="ITwoTerminal.TerminalB"/></param>
		/// <returns></returns>
		ISignalInformation GetVoltageDrop(ITwoTerminal component, bool voltageBA = true);

		#endregion
	}
}