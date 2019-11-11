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

		#endregion
	}
}
