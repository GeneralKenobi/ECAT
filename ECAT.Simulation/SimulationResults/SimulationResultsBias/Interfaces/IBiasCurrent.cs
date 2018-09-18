using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias
	{
		/// <summary>
		/// Private interface for classes capable of computing and caching currents through components, the purpose of the interface is
		/// to work with other private interfaces/classes computing and caching values of voltage, power, etc.
		/// </summary>
		private interface IBiasCurrent
		{
			#region Methods

			/// <summary>
			/// Gets current flowing through an <see cref="IResistor"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <param name="current"></param>
			/// <returns></returns>
			bool TryGetCurrent(IResistor resistor, out IPhasorDomainSignal current, bool voltageBA = true);

			/// <summary>
			/// Gets current flowing through an <see cref="ICapacitor"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="capacitor"></param>
			/// <paramref name="current"></paramref>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			bool TryGetCurrent(ICapacitor capacitor, out IPhasorDomainSignal current, bool voltageBA = true);

			/// <summary>
			/// Gets current produced by some <see cref="IActiveComponent"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <paramref name="current"></paramref>
			/// <param name="reverseDirection">If true, the direction of the current will be reversed (with the respect to
			/// the normal direction obtained in simulation)</param>
			/// <returns></returns>
			bool TryGetCurrent(int activeComponentIndex, out IPhasorDomainSignal current, bool reverseDirection = false);

			#endregion
		}
	}
}