using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Base implementation of Bipolar Junction Transistor
	/// </summary>
	public class Bjt : Transistor, IBjt
	{
		#region Public properties

		/// <summary>
		/// Input impedance
		/// </summary>
		public double Y11 => 1 / H11;

		/// <summary>
		/// Reverse-transfer admittance
		/// </summary>
		public double Y12 => -H12 / H11;

		/// <summary>
		/// Forward-transfer admittance
		/// </summary>
		public double Y21 => H21 / H11;

		/// <summary>
		/// Output admittance
		/// </summary>
		public double Y22 => H22 - H12 * H21 / H11;

		/// <summary>
		/// Input impedance
		/// </summary>
		public double H11 { get; set; } = 4 * 1e3;

		/// <summary>
		/// Reverse-voltage feedback
		/// </summary>
		public double H12 { get; set; } = 2.5 * 1e-4;

		/// <summary>
		/// Forward current gain
		/// </summary>
		public double H21 { get; set; } = 125;

		/// <summary>
		/// Output admittance
		/// </summary>
		public double H22 { get; set; } = 20 * 1e-6;

		/// <summary>
		/// Base-emitter voltage during saturation
		/// </summary>
		public double UBEForward { get; set; } = 0.6;

		/// <summary>
		/// Saturation collector-emitter voltage
		/// </summary>
		public double UCESaturation { get; set; } = 0.2;

		/// <summary>
		/// Beta coefficient of this BJT
		/// </summary>
		public double Beta { get; set; } = 100;

		#endregion
	}
}
