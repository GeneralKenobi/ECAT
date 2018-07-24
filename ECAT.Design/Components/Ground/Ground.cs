using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Implementation of <see cref="IGround"/>, all nodes connected with a ground have potential of 0V and may be connected with a wire
	/// </summary>
	public class Ground : OneTerminal, IGround
	{
		#region Public properties

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width { get; } = 50;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height { get; } = 50;

		#endregion
	}
}