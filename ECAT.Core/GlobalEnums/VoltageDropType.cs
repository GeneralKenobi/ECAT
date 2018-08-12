using System;

namespace ECAT.Core
{
	/// <summary>
	/// Denotes types of a voltage drop according to frequencies composing it
	/// </summary>
	[Flags]
	public enum VoltageDropType
    {
		/// <summary>
		/// The voltage drop contains DC component
		/// </summary>
		DC = 1,

		/// <summary>
		/// The voltage drop contains at least one AC component
		/// </summary>
		AC = 2,

		/// <summary>
		/// The voltage drop contains a single AC component
		/// </summary>
		SingleAC = 6,

		/// <summary>
		/// The voltage drop contains more than one AC component
		/// </summary>
		MultipleAC = 10,
    }
}