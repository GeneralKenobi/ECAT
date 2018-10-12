using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Interface for classes capable of computing and caching voltages across components, the purpose of the interface is
	/// to work with other interfaces/classes computing and caching values of current, power, etc.
	/// </summary>
	internal interface IVoltageSignalDB<T> where T : ISignalData
	{
		#region Methods

		/// <summary>
		/// Gets voltage drop of a node with respect to ground or null if unsuccessful and assigns it to <paramref name="voltage"/>.
		/// Returns true on success, false otherwise.
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <param name="voltage"></param>
		/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
		/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
		/// <returns></returns>
		bool TryGet(int nodeIndex, out T voltage, bool nodeToGround = true);

		/// <summary>
		/// Gets voltage drop between two nodes (with node A being treated as the reference node) or null if unsuccessful and
		/// assigns it to <paramref name="voltage"/>. Returns true on success, false otherwise.
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <param name="voltage"></param>
		/// <returns></returns>
		bool TryGet(int nodeAIndex, int nodeBIndex, out T voltage);

		/// <summary>
		/// Gets voltage drop across a <see cref="ITwoTerminal"/> component or null if unsuccessful and assigns it to
		/// <paramref name="voltage"/>. Returns true on success, false otherwise.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
		/// <param name="voltage"></param>
		/// <returns></returns>
		bool TryGet(ITwoTerminal component, out T voltage, bool voltageBA = true);

		#endregion
	}
}