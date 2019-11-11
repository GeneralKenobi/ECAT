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

		#endregion
	}
}