using ECAT.Core;
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
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="Signal"></param>
			public TimeDomainSignalInterpreter(ITimeDomainSignal Signal)
			{
				_Signal = Signal ?? throw new ArgumentNullException(nameof(Signal));
			}

			#endregion

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
			public double Maximum() => _Signal.FinalWaveform.Max();

			/// <summary>
			/// Calculates and returns the minimum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Minimum() => _Signal.FinalWaveform.Min();

			/// <summary>
			/// Calculates and returns the root-mean-square value of the signal
			/// </summary>
			/// <returns></returns>
			public double RMS() => Math.Sqrt(_Signal.FinalWaveform.Sum((x) => Math.Pow(x, 2)) / _Signal.FinalWaveform.Count());

			/// <summary>
			/// Returns the average value of the signal
			/// </summary>
			/// <returns></returns>
			public double Average() => _Signal.FinalWaveform.Average();

			#endregion
		}
	}
}