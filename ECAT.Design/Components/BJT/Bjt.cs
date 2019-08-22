using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	public class Bjt : ThreeTerminal, IBjt
	{
		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>
		/// </summary>
		protected override Complex _TerminalAShift { get; } = new Complex(-100, 0);

		/// <summary>
		/// The shift assigned to <see cref="TerminalB"/>
		/// </summary>
		protected override Complex _TerminalBShift { get; } = new Complex(100, -100);

		/// <summary>
		/// The shift assigned to <see cref="TerminalC"/>
		/// </summary>
		protected override Complex _TerminalCShift { get; } = new Complex(100, 100);

		#endregion

		#region Public properties

		/// <summary>
		/// Input impedance
		/// </summary>
		public double Y11 { get; set; }

		/// <summary>
		/// Reverse-transfer admittance
		/// </summary>
		public double Y12 { get; set; }

		/// <summary>
		/// Forward-transfer admittance
		/// </summary>
		public double Y21 { get; set; }

		/// <summary>
		/// Output admittance
		/// </summary>
		public double Y22 { get; set; }

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width { get; } = 200;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height { get; } = 200;

		/// <summary>
		/// Cutoff base-emitter voltage
		/// </summary>
		public double UBECutoff { get; set; } = 0.6;

		/// <summary>
		/// Saturation collector-emitter voltage
		/// </summary>
		public double UCESaturation { get; set; } = 0.2;

		#endregion
	}
}
