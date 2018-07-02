using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

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
	}
}