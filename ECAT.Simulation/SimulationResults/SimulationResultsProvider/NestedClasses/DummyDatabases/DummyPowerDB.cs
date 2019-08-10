using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsProvider
	{
		/// <summary>
		/// Dummy power database - always returns null.
		/// </summary>
		public class DummyPowerDB : IPowerDB
		{
			#region Methods

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="resistor"></param>
			/// <returns></returns>
			public ISignalInformation Get(IResistor resistor, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="capacitor"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICapacitor capacitor, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="inductor"></param>
			/// <returns></returns>
			public ISignalInformation Get(IInductor inductor, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICurrentSource currentSource, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IDCVoltageSource voltageSource, bool voltageBA) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IACVoltageSource voltageSource, bool voltageBA) => null;

			#endregion
		}
	}
}