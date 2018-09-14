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
			/// <param name="resistor"></param>
			/// <param name="current"></param>
			/// <returns></returns>
			bool TryGetCurrent(IResistor resistor, out IPhasorDomainSignal current);

			/// <summary>
			/// Gets current flowing through an <see cref="ICapacitor"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// <param name="capacitor"></param>
			/// <paramref name="current"></paramref>
			/// <returns></returns>
			bool GetCurrent(ICapacitor capacitor, out IPhasorDomainSignal current);

			/// <summary>
			/// Gets current produced by some <see cref="IActiveComponent"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <paramref name="current"></paramref>
			/// <returns></returns>
			bool GetCurrent(int activeComponentIndex, out IPhasorDomainSignal current);

			#endregion
		}
	}
}