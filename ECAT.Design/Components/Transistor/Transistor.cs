using ECAT.Core;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of a base class for Transistors.
	/// </summary>
	public abstract class Transistor : ThreeTerminal, ITransistor
	{
		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>
		/// </summary>
		protected override Complex _TerminalAShift { get; } = new Complex(-100, 0);

		/// <summary>
		/// The shift assigned to <see cref="TerminalB"/>
		/// </summary>
		protected override Complex _TerminalBShift { get; } = new Complex(50, 100);

		/// <summary>
		/// The shift assigned to <see cref="TerminalC"/>
		/// </summary>
		protected override Complex _TerminalCShift { get; } = new Complex(50, -100);

		#endregion

		#region Public properties
		
		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width { get; } = 200;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height { get; } = 200;

		#endregion
	}
}
