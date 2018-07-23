using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for voltage sources
	/// </summary>
    public interface IVoltageSource : ITwoTerminal
    {
		#region Properties

		/// <summary>
		/// Current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		double ProducedVoltage { get; set; }

		#endregion
	}
}
