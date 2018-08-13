using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Default of implementation of <see cref="IOneTerminal"/>
	/// </summary>
	public abstract class OneTerminal : BaseComponent, IOneTerminal
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public OneTerminal()
		{
			Terminal = new Terminal(new PlanePosition(Complex.Zero, _TerminalShift));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="Terminal"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalShift { get; } = Complex.Zero;

		#endregion

		#region Public properties

		/// <summary>
		/// The terminal of this <see cref="IOneTerminal"/>
		/// </summary>
		public ITerminal Terminal { get; }

		/// <summary>
		/// Input admittance of <see cref="Terminal"/>
		/// </summary>
		public Complex Admittance { get ; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Rotates all terminals by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected override void RotateTerminals(double degrees) => Terminal.Position.RotationAngle += degrees;

		/// <summary>
		/// Assigns positions to all <see cref="ITerminal"/>s
		/// </summary>
		protected override void UpdateAbsoluteTerminalPositions() => Terminal.Position.Absolute = new Complex(Center.X, Center.Y);

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ITerminal> GetTerminals()
		{
			yield return Terminal;
		}

		#endregion
	}
}