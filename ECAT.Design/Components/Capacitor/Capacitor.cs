using System;
using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class implementing capacitors, standard implementation of <see cref="ICapacitor"/>
	/// </summary>
	public class Capacitor : TwoTerminal, ICapacitor
    {
		#region Public properties

		/// <summary>
		/// Capacitance of this <see cref="Capacitor"/>
		/// </summary>
		public double Capacitance { get; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of this <see cref="Capacitor"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => new Complex(0, 2 * Math.PI * Capacitance);

		#endregion
	}
}