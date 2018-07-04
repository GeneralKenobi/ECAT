using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all two-terminal components
	/// </summary>
    public abstract class TwoTerminal : BaseComponent
    {
		#region Public properties

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public PartialNode TerminalA { get; set; } = new PartialNode();

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public PartialNode TerminalB { get; set; } = new PartialNode();

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width => 100;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height => 50;

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="PartialNode"/>s
		/// </summary>
		protected override void UpdatePartialNodePositions()
		{			
			TerminalA.Position = new PlanePosition(Center.X, Center.Y, -Width / 2, 0);
			TerminalB.Position = new PlanePosition(Center.X, Center.Y, Width / 2, 0);			

			TerminalA.Position.Shift.Rotate(RotationAngle);
			TerminalB.Position.Shift.Rotate(RotationAngle);
		}

		#endregion
	}
}