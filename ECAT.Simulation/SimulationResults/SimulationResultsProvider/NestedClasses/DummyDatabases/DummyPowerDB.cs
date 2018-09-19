using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsProvider
	{
		/// <summary>
		/// Dummy power database - always returns null.
		/// </summary>
		private class DummyPowerDB : IPowerDB
		{
			#region Methods

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="resistor"></param>
			/// <returns></returns>
			public ISignalInformation Get(IResistor resistor) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICurrentSource currentSource) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IVoltageSource voltageSource) => null;

			/// <summary>
			/// Returns null
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IACVoltageSource voltageSource) => null;

			#endregion
		}
	}
}