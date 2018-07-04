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

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="PartialNode"/>s
		/// </summary>
		protected override void UpdatePartialNodePositions()
		{
			TerminalA.Position = new PlanePosition(Center.Re, Center.Im, -Width / 2, 0);
			TerminalB.Position = new PlanePosition(Center.Re, Center.Im, Width / 2, 0);
		}

		#endregion
	}
}