using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all two-terminal components
	/// </summary>
    public abstract class TwoTerminal : BaseComponent, ITwoTerminal
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public TwoTerminal()
		{
			TerminalA = new Terminal(new PlanePosition(Complex.Zero, _TerminalAShift), TerminalPotentialChangedCallback);
			TerminalB = new Terminal(new PlanePosition(Complex.Zero, _TerminalBShift), TerminalPotentialChangedCallback);
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalAShift => new Complex(-Width / 2, 0);

		/// <summary>
		/// The shift assigned to <see cref="TerminalB"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalBShift => new Complex(Width / 2, 0);

		#endregion

		#region Public properties		

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width => 200;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height => 100;

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public ITerminal TerminalB { get; }		

		/// <summary>
		/// Admittance between terminals A and B
		/// </summary>
		public Complex Admittance { get; set; }

		/// <summary>
		/// Voltage drop between <see cref="TerminalB"/> and <see cref="TerminalA"/> (VB - VA)
		/// </summary>
		public double VoltageBA => TerminalB.Potential == null || TerminalA.Potential == null ? 
			0 : TerminalB.Potential.Value - TerminalA.Potential.Value;

		/// <summary>
		/// Current flowing from <see cref="TerminalA"/> to <see cref="TerminalB"/>, may be overriden if a specific component'
		/// current can't be determined from its voltage and admittance (eg. voltage source)
		/// </summary>
		public virtual double CurrentBA => -VoltageBA * Admittance.Real;

		#endregion

		#region Private methods

		/// <summary>
		/// Callback for when value of any <see cref="ITerminal"/> in this <see cref="TwoTerminal"/> changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TerminalPotentialChangedCallback(object sender, EventArgs e) =>
			InvokePropertyChanged(nameof(VoltageBA), nameof(CurrentBA));

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="ITerminal"/>s
		/// </summary>
		protected override void UpdateAbsoluteTerminalPositions()
		{
			TerminalA.Position.Absolute = new Complex(Center.X, Center.Y);
			TerminalB.Position.Absolute = new Complex(Center.X, Center.Y);
		}

		/// <summary>
		/// Rotates all partial nodes by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected override void RotateTerminals(double degrees)
		{
			TerminalA.Position.RotationAngle += degrees;
			TerminalB.Position.RotationAngle += degrees;
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
		};

		#endregion
	}
}