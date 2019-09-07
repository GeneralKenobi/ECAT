using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for Bipolar Junction Transistors. Terminal A maps to base, terminal B maps to collector and terminal C maps to emitter.
	/// </summary>
	public interface IBjt : ITransistor
	{
		#region Properties

		/// <summary>
		/// Input admittance
		/// </summary>
		double Y11 { get; }

		/// <summary>
		/// Reverse-transfer admittance
		/// </summary>
		double Y12 { get; }

		/// <summary>
		/// Forward-transfer admittance
		/// </summary>
		double Y21 { get; }

		/// <summary>
		/// Output admittance
		/// </summary>
		double Y22 { get; }

		/// <summary>
		/// Input impedance
		/// </summary>
		double H11 { get; set; }

		/// <summary>
		/// Reverse-voltage feedback
		/// </summary>
		double H12 { get; set; }

		/// <summary>
		/// Forward current gain
		/// </summary>
		double H21 { get; set; }

		/// <summary>
		/// Output admittance
		/// </summary>
		double H22 { get; set; }

		/// <summary>
		/// Base-emitter voltage during saturation
		/// </summary>
		double UBEForward { get; set; }

		/// <summary>
		/// Saturation collector-emitter voltage
		/// </summary>
		double UCESaturation { get; set; }

		/// <summary>
		/// Beta coefficient of this BJT
		/// </summary>
		double Beta { get; set; }

		#endregion
	}
}