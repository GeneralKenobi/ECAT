﻿using CSharpEnhanced.CoreInterfaces;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing information about a signal - voltage drop or current flow
	/// </summary>
	public interface ISignalInformation : ISignal, IDeepCopy<ISignalInformation>
	{
		#region Properties

		/// <summary>
		/// True if the direction of signal was inverted (with respect to assumed directions) to present <see cref="Maximum"/> as a
		/// positive number
		/// </summary>
		bool InvertedDirection { get; }

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
		/// The type of the signal
		/// </summary>
		SignalType Type { get; }

		#endregion
	}
}