using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias
	{
		/// <summary>
		/// Private interface for classes capable of computing and caching voltages across components, the purpose of the interface is
		/// to work with other private interfaces/classes computing and caching values of current, power, etc.
		/// </summary>
		private interface IBiasVoltage
		{
			#region Methods

			/// <summary>
			/// Gets voltage drop of a node with respect to ground or returns null if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <returns></returns>
			IPhasorDomainSignal GetVoltageDrop(int nodeIndex);

			/// <summary>
			/// Gets voltage drop between two nodes (with node A being treated as the reference node) or returns null
			/// if unsuccessful
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			IPhasorDomainSignal GetVoltageDrop(int nodeAIndex, int nodeBIndex);

			/// <summary>
			/// Gets voltage drop across a <see cref="ITwoTerminal"/> component
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <see cref="ITwoTerminal.TerminalA"/>, if false from <see cref="ITwoTerminal.TerminalA"/> to
			/// <see cref="ITwoTerminal.TerminalB"/></param>
			/// <returns></returns>
			IPhasorDomainSignal GetVoltageDrop(ITwoTerminal component, bool voltageBA = true);

			#endregion
		}
	}
}