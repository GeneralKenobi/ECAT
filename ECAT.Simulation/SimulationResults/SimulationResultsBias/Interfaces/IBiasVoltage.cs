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
			/// Gets voltage drop of a node with respect to ground or null if unsuccessful and assigns it to <paramref name="voltage"/>.
			/// Returns true on success, false otherwise.
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="voltage"></param>
			/// <returns></returns>
			bool GetVoltageDrop(int nodeIndex, out IPhasorDomainSignal voltage);

			/// <summary>
			/// Gets voltage drop between two nodes (with node A being treated as the reference node) or null if unsuccessful and
			/// assigns it to <paramref name="voltage"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <param name="voltage"></param>
			/// <returns></returns>
			bool GetVoltageDrop(int nodeAIndex, int nodeBIndex, out IPhasorDomainSignal voltage);

			/// <summary>
			/// Gets voltage drop across a <see cref="ITwoTerminal"/> component or null if unsuccessful and assigns it to
			/// <paramref name="voltage"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <param name="voltage"></param>
			/// <returns></returns>
			bool GetVoltageDrop(ITwoTerminal component, out IPhasorDomainSignal voltage, bool voltageBA = true);

			#endregion
		}
	}
}