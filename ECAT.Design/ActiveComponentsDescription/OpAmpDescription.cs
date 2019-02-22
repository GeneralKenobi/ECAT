using ECAT.Core;

namespace ECAT.Design
{
	public class OpAmpDescription : ComponentDescription, IOpAmpDescription
	{
		#region Public properties

		/// <summary>
		/// Positive supply voltage of the described <see cref="IOpAmp"/>
		/// </summary>
		public double PositiveSupplyVoltage { get; set; }

		/// <summary>
		/// Positive supply voltage of the described <see cref="IOpAmp"/>
		/// </summary>
		public double NegativeSupplyVoltage { get; set; }

		/// <summary>
		/// Open loop gain of the described <see cref="IOpAmp"/>
		/// </summary>
		public double OpenLoopGain { get; set; }

		#endregion
	}
}