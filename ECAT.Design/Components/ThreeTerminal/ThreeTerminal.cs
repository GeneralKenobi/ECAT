using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Design
{
	public abstract class ThreeTerminal : BaseComponent, IThreeTerminal
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ThreeTerminal()
		{
			TerminalA = new Terminal(new PlanePosition(Complex.Zero, _TerminalAShift));
			TerminalB = new Terminal(new PlanePosition(Complex.Zero, _TerminalBShift));
			TerminalC = new Terminal(new PlanePosition(Complex.Zero, _TerminalCShift));
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>, override to provide custom value
		/// </summary>
		protected abstract Complex _TerminalAShift { get; }

		/// <summary>
		/// The shift assigned to <see cref="TerminalB"/>, override to provide custom value
		/// </summary>
		protected abstract Complex _TerminalBShift { get; }

		/// <summary>
		/// The shift assigned to <see cref="TerminalC"/>, override to provide custom value
		/// </summary>
		protected abstract Complex _TerminalCShift { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// One of the terminals in this three-terminal
		/// </summary>
		public ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this three-terminal
		/// </summary>
		public ITerminal TerminalB { get; }

		/// <summary>
		/// One of the terminals in this three-terminal
		/// </summary>
		public ITerminal TerminalC { get; }

		/// <summary>
		/// The admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// </summary>
		public Complex AdmittanceAB { get; set; }

		/// <summary>
		/// The admittance between <see cref="TerminalA"/> and <see cref="TerminalC"/>
		/// </summary>
		public Complex AdmittanceAC { get; set; }

		/// <summary>
		/// The admittance between <see cref="TerminalB"/> and <see cref="TerminalC"/>
		/// </summary>
		public Complex AdmittanceBC { get; set; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalB"/> and <see cref="TerminalA"/>
		/// </summary>
		public double VoltageBA { get; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalC"/> and <see cref="TerminalA"/>
		/// </summary>
		public double VoltageCA { get; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalC"/> and <see cref="TerminalB"/>
		/// </summary>
		public double VoltageCB { get; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="ITerminal"/>s
		/// </summary>
		protected override void UpdateAbsoluteTerminalPositions()
		{
			TerminalA.Position.Absolute = new Complex(Center.X, Center.Y);
			TerminalB.Position.Absolute = new Complex(Center.X, Center.Y);
			TerminalC.Position.Absolute = new Complex(Center.X, Center.Y);
		}

		/// <summary>
		/// Rotates all partial nodes by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected override void RotateTerminals(double degrees)
		{
			TerminalA.Position.RotationAngle += degrees;
			TerminalB.Position.RotationAngle += degrees;
			TerminalC.Position.RotationAngle += degrees;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		public override List<ITerminal> GetTerminals() => new List<ITerminal>()
		{
			TerminalA,
			TerminalB,
			TerminalC,
		};

		#endregion
	}
}
