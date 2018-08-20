using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing information about a voltage drop between two nodes
	/// </summary>
	public interface IVoltageDropInformation
    {
		#region Properties

		/// <summary>
		/// True if the direction of voltage drops was inverted to present <see cref="Maximum"/> as a positive number
		/// </summary>
		bool InvertedDirection { get; }

		/// <summary>
		/// DC voltage drop
		/// </summary>
		double DC { get; }

		/// <summary>
		/// The maximum voltage drop that may occur
		/// </summary>
		double Maximum { get; }

		/// <summary>
		/// The minimum voltage drop that may occur
		/// </summary>
		double Minimum { get; }

		/// <summary>
		/// RMS value of this voltage drop
		/// </summary>
		double RMS { get; }

		/// <summary>
		/// List with all AC waveforms (voltage drops) adding to the total voltage drop
		/// </summary>
		IEnumerable<KeyValuePair<double, Complex>> ComposingACWaveforms { get; }

		/// <summary>
		/// The type of the voltage drop
		/// </summary>
		VoltageDropType Type { get; }

		#endregion
	}
}