using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	public abstract class OneTerminal : BaseComponent, IOneTerminal
	{
		public ITerminal Terminal { get; } = new Terminal();

		public Complex Admittance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalShift => new Complex(0, -Width/2);

		#endregion
	}
}
