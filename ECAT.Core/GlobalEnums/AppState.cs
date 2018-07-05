using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Defines states the app may be in
	/// </summary>
    public enum AppState
    {
		/// <summary>
		/// App is in idle state - nothing is going on
		/// </summary>
		Idle = 0,

		/// <summary>
		/// User is currently adding new components
		/// </summary>
		AddingComponents = 1,

		/// <summary>
		/// User is currently placing a wire
		/// </summary>
		AddingWire = 2,
    }
}
