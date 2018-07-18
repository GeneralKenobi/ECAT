using System;

namespace ECAT.Core
{
	/// <summary>
	/// Flags that denote modifier keys that may have been present with a key press
	/// </summary>
	[Flags]
	public enum KeyModifiers
    {
		/// <summary>
		/// Shift key
		/// </summary>
		Shift = 1,

		/// <summary>
		/// Ctrl key
		/// </summary>
		Ctrl = 2,

		/// <summary>
		/// Alt key
		/// </summary>
		Alt = 4
    }
}