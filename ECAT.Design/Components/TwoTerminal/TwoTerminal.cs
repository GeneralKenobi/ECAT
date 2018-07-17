using CSharpEnhanced.Maths;
using ECAT.Core;
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
			TerminalA = new PlanePosition(Complex.Zero, _TerminalAShift);
			TerminalB = new PlanePosition(Complex.Zero, _TerminalBShift);
		}

		#endregion

		#region Public properties		

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width => 100;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height => 50;

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public IPlanePosition TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public IPlanePosition TerminalB { get; }

		/// <summary>
		/// The admittance between the two terminals
		/// </summary>
		public Complex Admittance { get; set; }

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

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="PartialNode"/>s
		/// </summary>
		protected override void UpdateAbsolutePartialNodePositions()
		{
			TerminalA.Absolute = new Complex(Center.X, Center.Y);
			TerminalB.Absolute = new Complex(Center.X, Center.Y);
		}

		/// <summary>
		/// Rotates all partial nodes by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected override void RotatePartialNodes(double degrees)
		{
			TerminalA.RotationAngle += degrees;
			TerminalB.RotationAngle += degrees;
		}

		#endregion
	}
}