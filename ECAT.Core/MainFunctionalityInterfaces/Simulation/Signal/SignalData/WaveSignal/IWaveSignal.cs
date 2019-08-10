using CSharpEnhanced.CoreInterfaces;
using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a signal calculated in frequency domain (transfer function charcteristics)
	/// </summary>
	public interface IWaveSignal<T>: ISignalData
	{
		#region Properties

		/// <summary>
		/// Number of samples in this signal
		/// </summary>
		int Samples { get; }

		/// <summary>
		/// Start value on X axis
		/// </summary>
		double StartSample { get; }

		/// <summary>
		/// Step on X axis
		/// </summary>
		double Step { get; }

		/// <summary>
		/// List with values
		/// </summary>
		IEnumerable<T> Waveform { get; }
		
		#endregion
	}
}