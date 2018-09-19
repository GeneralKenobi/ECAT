using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsProvider
	{
		/// <summary>
		/// Dummy voltage database - always returns null.
		/// </summary>
		private class DummyVoltageDB : IVoltageDB
		{
			#region Methods

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeIndex, bool nodeToGround = true) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeAIndex, int nodeBIndex) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <returns></returns>
			public ISignalInformation Get(ITwoTerminal component, bool voltageBA = true) => null;

			#endregion
		}
	}
}