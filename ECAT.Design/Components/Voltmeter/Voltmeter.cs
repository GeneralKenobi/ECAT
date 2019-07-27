using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of an IVoltmeter
	/// </summary>
	public class Voltmeter : TwoTerminal, IVoltmeter
	{
		#region Protected methods

		/// <summary>
		/// Returns admittance of a voltmeter (always 0 - infinite resistance)
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => 0;

		#endregion
	}
}
