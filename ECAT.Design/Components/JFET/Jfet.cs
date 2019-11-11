using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Class representing a Junction Field Effect Transistor
	/// </summary>
	public class Jfet : Transistor, IJfet
	{
		#region Public properties

		/// <summary>
		/// Gate
		/// </summary>
		public double RGS { get; set; } = 1e+9;

		/// <summary>
		/// Small-signal output resistance
		/// </summary>
		public double RDS { get; set; } = 1e+6;

		/// <summary>
		/// Transconductance
		/// </summary>
		public double GM { get; set; } = 0.0075;

		#endregion
	}
}
