﻿using ECAT.Core;
using System;
using System.Linq;

namespace ECAT.Simulation
{
	public partial class TimeDomainSignal : ITimeDomainSignal
	{
		/// <summary>
		/// Class capable of interpreting <see cref="ITimeDomainSignal"/> and calculating its characteristic values
		/// </summary>
		private class TimeDomainSignalInterpreter : ISignalDataInterpreter
		{
			#region Private properties

			/// <summary>
			/// Signal being interpreted
			/// </summary>
			private ITimeDomainSignal _Signal { get; }

			#endregion

			#region Public methods

			/// <summary>
			/// Calculates and returns the maximum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Maximum() => _Signal.InstantenousValues.Max();

			/// <summary>
			/// Calculates and returns the minimum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Minimum() => _Signal.InstantenousValues.Min();

			/// <summary>
			/// Calculates and returns the root-mean-square value of the signal
			/// </summary>
			/// <returns></returns>
			public double RMS() => Math.Sqrt(_Signal.InstantenousValues.Sum((x) => Math.Pow(x, 2)) / _Signal.InstantenousValues.Count());

			#endregion
		}
	}
}