using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for all two-terminal components
	/// </summary>
    public class TwoTerminal : BaseComponent
    {
		#region Public properties

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public Node TerminalA { get; set; } = Node.Factory.Construct();

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public Node TerminalB { get; set; } = Node.Factory.Construct();

		#endregion
	}
}