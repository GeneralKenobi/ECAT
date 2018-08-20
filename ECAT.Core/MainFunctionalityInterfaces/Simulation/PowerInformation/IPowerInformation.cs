﻿namespace ECAT.Core
{
	/// <summary>
	/// Contains information about power
	/// </summary>
	public interface IPowerInformation
    {
		#region Properties

		/// <summary>
		/// Average value
		/// </summary>
		double Average { get; }

		/// <summary>
		/// Maximum instantenous power lost
		/// </summary>
		double MaximumLost { get; }
		
		/// <summary>
		/// Maximum instantenous power supplied
		/// </summary>
		double MaximumSupplied { get; }

		#endregion
	}
}