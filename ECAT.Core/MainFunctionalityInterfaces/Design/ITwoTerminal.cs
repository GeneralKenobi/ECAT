using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Core
{
    public interface ITwoTerminal : IBaseComponent
    {
		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		ITerminal TerminalB { get; }

		/// <summary>
		/// Admittance between terminals A and B
		/// </summary>
		Complex Admittance { get; }
	}
}