using System;
using System.Collections.Generic;
using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class implementing capacitors, standard implementation of <see cref="ICapacitor"/>
	/// </summary>
	public class Capacitor : TwoTerminal, ICapacitor
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public Capacitor() : base(new string[] { QuantityNames.Singleton.VoltageCap, QuantityNames.Singleton.CurrentCap}) { }

		#endregion

		#region Public properties

		/// <summary>
		/// Capacitance of this <see cref="Capacitor"/>
		/// </summary>
		public double Capacitance { get; set; } = IoC.Resolve<IDefaultValues>().DefaultCapacitorCapacitance;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of this <see cref="Capacitor"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => new Complex(0, 2 * Math.PI * frequency * Capacitance);

		/// <summary>
		/// Returns capacitor's info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			yield return GetVoltageInfo(_VoltageDrop);
			yield return GetCurrentInfo(IoC.Resolve<ISimulationResults>().GetCurrent(_VoltageDrop, this));
		}

		#endregion
	}
}