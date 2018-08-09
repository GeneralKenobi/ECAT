using System.Collections.Generic;
using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Implementation of <see cref="IGround"/>, all nodes connected with a ground have potential of 0V and may be connected with a wire
	/// </summary>
	public class Ground : OneTerminal, IGround
	{
		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="Terminal"/>, override to provide custom value
		/// </summary>
		protected override Complex _TerminalShift { get; } = new Complex(0, 25);

		#endregion

		#region Public properties

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width { get; } = 100;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height { get; } = 100;

		#endregion

		#region Public methods

		/// <summary>
		/// Returns the info that is to be presented, for example, on pointer over. It should include voltage drop(s) across the element,
		/// current(s) through the element, etc.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetComponentInfo() => null;		

		#endregion
	}
}