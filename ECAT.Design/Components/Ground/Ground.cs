using System.Collections.Generic;
using System.Linq;
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

		#region Protected methods

		/// <summary>
		/// Returns info for grounds, be default it's
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo() => Enumerable.Empty<IEnumerable<string>>();

		#endregion
	}
}