using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsProvider
	{
		/// <summary>
		/// Dummy current database - always returns null.
		/// </summary>
		private class DummyCurrentDB : ICurrentDB
		{
			#region Methods

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="resistor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation Get(IResistor resistor, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="capacitor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation Get(ICapacitor capacitor, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
			/// by convention for the specific element (obtained during simulation)</param>
			/// <returns></returns>
			public ISignalInformation Get(int activeComponentIndex, bool reverseDirection) => null;


			/// <summary>
			/// Gets information about current flowing through an <see cref="IInductor"/> or null if unsuccessful
			/// </summary>		
			/// <param name="inductor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation Get(IInductor inductor, bool voltageBA) => null;

			#endregion
		}
	}
}