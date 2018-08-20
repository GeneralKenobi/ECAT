using System;

namespace ECAT.Core
{
	/// <summary>
	/// Denotes types of a signal according to frequencies composing it
	/// </summary>
	[Flags]
	public enum SignalType
    {
		/// <summary>
		/// No signal is present
		/// </summary>
		Empty = 0,

		/// <summary>
		/// The signal contains a DC component
		/// </summary>
		DC = 1,

		/// <summary>
		/// The signal contains at least one AC component
		/// </summary>
		AC = 2,

		/// <summary>
		/// The signal contains a single AC component
		/// </summary>
		SingleAC = 6,

		/// <summary>
		/// The signal contains more than one AC component
		/// </summary>
		MultipleAC = 10,
    }
}