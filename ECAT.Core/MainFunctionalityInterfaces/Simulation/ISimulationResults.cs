namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that manages and analyses results computed by <see cref="ISimulationManager"/>
	/// </summary>
	public interface ISimulationResults
    {
		#region Methods

		/// <summary>
		/// Gets a voltage drop of a node with respect to ground
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <returns></returns>
		bool TryGetVoltageDrop(int nodeIndex, out IVoltageDropInformation voltageDrop);

		/// <summary>
		/// Gets information on voltage drop between two nodes (with node A being treated as the reference node). If the node
		/// indexes exceed currently held nodes count null is assigned to <paramref name="voltageDrop"/> and false is returned
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <returns></returns>
		bool TryGetVoltageDrop(int nodeAIndex, int nodeBIndex, out IVoltageDropInformation voltageDrop);

		#endregion
	}
}