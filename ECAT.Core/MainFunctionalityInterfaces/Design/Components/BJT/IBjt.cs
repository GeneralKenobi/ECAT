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
		/// Input impedance
		/// </summary>
		double Y11 { get; set; }

		/// <summary>
		/// Reverse-transfer admittance
		/// </summary>
		double Y12 { get; set; }

		/// <summary>
		/// Forward-transfer admittance
		/// </summary>
		double Y21 { get; set; }

		/// <summary>
		/// Output admittance
		/// </summary>
		double Y22 { get; set; }

		/// <summary>
		/// Cutoff base-emitter voltage
		/// </summary>
		double UBECutoff { get; set; }

		/// <summary>
		/// Saturation collector-emitter voltage
		/// </summary>
		double UCESaturation { get; set; }

		#endregion
	}
}