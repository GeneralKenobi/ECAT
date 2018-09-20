﻿using ECAT.Core;
using System;
using System.Linq;

namespace ECAT.Simulation
{
	public partial class PhasorDomainSignal
	{
		/// <summary>
		/// Class capable of interpreting <see cref="IPhasorDomainSignal"/> and calculating its characteristic values
		/// </summary>
		private class PhasorDomainSignalInterpreter : ISignalDataInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="Signal">Signal being interpreted</param>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="Signal"/> is null</exception>
			public PhasorDomainSignalInterpreter(IPhasorDomainSignal Signal)
			{
				_Signal = Signal ?? throw new ArgumentNullException(nameof(Signal));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Signal that is interpreted
			/// </summary>
			private IPhasorDomainSignal _Signal { get; }

			#endregion

			#region Public methods

			/// <summary>
			/// Calculates and returns the maximum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Maximum() => _Signal.DC + _Signal.Phasors.Sum((phasor) => phasor.Value.Magnitude);

			/// <summary>
			/// Calculates and returns the minimum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Minimum() => _Signal.DC - _Signal.Phasors.Sum((phasor) => phasor.Value.Magnitude);

			/// <summary>
			/// Calculates and returns the root-mean-square value of the signal
			/// </summary>
			/// <returns></returns>
			public double RMS() =>
				Math.Sqrt(Math.Pow(_Signal.DC, 2) + _Signal.Phasors.Sum((x) => Math.Pow(x.Value.Magnitude / Math.Sqrt(2), 2)));

			/// <summary>
			/// Returns the average value of the signal
			/// </summary>
			/// <returns></returns>
			public double Average() => _Signal.DC;
			// Average value is equal to DC (sine waves have average value equal to 0)

			#endregion
		}
	}
}