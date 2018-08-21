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
			/// Average value
			/// </summary>
			public double Average { get; set; }

			/// <summary>
			/// Maximum instantenous power loss
			/// </summary>
			public double MaximumLost { get; set; }

			/// <summary>
			/// Maximum instantenous power supplied
			/// </summary>
			public double MaximumSupplied { get; set; }

			#endregion
		}
	}
}