using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Class for a resistor in circuit design
	/// </summary>
    public class Resistor : TwoTerminal
    {

		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Resistor()
		{
			Handle.Shift = new Coord(-Width / 2, -Height / 2);
		}

		#endregion

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width => 75;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height => 25;
	}
}
