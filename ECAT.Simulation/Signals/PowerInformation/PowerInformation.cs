using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationManager
	{
		/// <summary>
		/// Standard implementation of <see cref="IPowerInformation"/>, presents information about a power loss/delivery
		/// </summary>
		private class PowerInformation : IPowerInformation
		{
			#region Public properties

			/// <summary>
			/// Average value, negative power is supplied, positive power is dissipated
			/// </summary>
			public double Average { get; set; }

			/// <summary>
			/// Determines whether power is dissipated/supplied by the element
			/// </summary>
			public PowerType AveragePowerType =>
				// Average equal to 0 is no power, average greater than 0 is dissipated and average smaller than 0 is supplied
				Average == 0 ? PowerType.None : Average > 0 ? PowerType.Dissipated : PowerType.Supplied;

			/// <summary>
			/// Maximum instantenous power value
			/// </summary>
			public double Maximum { get; set; }

			/// <summary>
			/// Minimum instantenous power value 
			/// </summary>
			public double Minimum { get; set; }

			#endregion
		}
	}
}