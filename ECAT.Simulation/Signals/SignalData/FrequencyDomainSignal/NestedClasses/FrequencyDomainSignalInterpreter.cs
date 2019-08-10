using ECAT.Core;
using System;
using System.Linq;

namespace ECAT.Simulation
{
	public partial class FrequencyDomainSignal
	{
		/// <summary>
		/// Class capable of interpreting <see cref="IFrequencyDomainSignal"/> and calculating its characteristic values
		/// </summary>
		private class FrequencyDomainSignalInterpreter : ISignalDataInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="Signal"></param>
			public FrequencyDomainSignalInterpreter(IFrequencyDomainSignal Signal)
			{
				_Signal = Signal ?? throw new ArgumentNullException(nameof(Signal));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Signal being interpreted
			/// </summary>
			private IFrequencyDomainSignal _Signal { get; }

			#endregion

			#region Public methods

			/// <summary>
			/// Calculates and returns the maximum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Maximum() => _Signal.Waveform.Max((x) => x.Magnitude);

			/// <summary>
			/// Calculates and returns the minimum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Minimum() => _Signal.Waveform.Min((x) => x.Magnitude);

			/// <summary>
			/// Calculates and returns the root-mean-square value of the signal
			/// </summary>
			/// <returns></returns>
			public double RMS() => 0;

			/// <summary>
			/// Returns the average value of the signal
			/// </summary>
			/// <returns></returns>
			public double Average() => 0;

			#endregion
		}
	}
}