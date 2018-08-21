﻿using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing information about a signal - voltage drop or current flow
	/// </summary>
	public interface ISignalInformation
    {
		#region Properties

		/// <summary>
		/// True if the direction of signal was inverted (with respect to assumed directions) to present <see cref="Maximum"/> as a
		/// positive number
		/// </summary>
		bool InvertedDirection { get; }

		/// <summary>
		/// DC component of the signal
		/// </summary>
		double DC { get; }

		/// <summary>
		/// The maximum instantenous signal value that may occur
		/// </summary>
		double Maximum { get; }

		/// <summary>
		/// The minimum instantenous signal value that may occur
		/// </summary>
		double Minimum { get; }

		/// <summary>
		/// RMS value of this signal
		/// </summary>
		double RMS { get; }

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		IEnumerable<KeyValuePair<double, Complex>> ComposingPhasors { get; }

		/// <summary>
		/// The type of the signal
		/// </summary>
		SignalType Type { get; }

		#endregion
	}
}