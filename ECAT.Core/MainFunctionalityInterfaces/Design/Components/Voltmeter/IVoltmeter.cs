using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for voltmeters - components with infinite resistance that are used to construct voltage drop characteristics
	/// </summary>
	public interface IVoltmeter : ITwoTerminal
	{
		#region Properties

		/// <summary>
		/// The ID number of this Voltmeter (used to differentiate between generated voltage plots)
		/// </summary>
		int ID { get; }

		#endregion
	}
}