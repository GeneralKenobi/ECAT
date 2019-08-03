using System;
using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class implementing inductors, standard implementation of <see cref="ICapacitor"/>
	/// </summary>
	[DisplayVoltageInfo(nameof(TerminalA), nameof(TerminalB), 0, "Voltage")]
	[DisplayCurrentInfo(sectionIndex: 1)]
	[DisplayPowerInfo(sectionIndex: 2)]
	public class Inductor : TwoTerminal, IInductor
    {
		#region Public properties

		/// <summary>
		/// Inductance of this <see cref="IInductor"/>
		/// </summary>
		public double Inductance { get; set; } = IoC.Resolve<IDefaultValues>().DefaultInductorInductance;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of this <see cref="IInductor"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => new Complex(0, 1 / (2 * Math.PI * frequency * Inductance));

		#endregion
	}
}