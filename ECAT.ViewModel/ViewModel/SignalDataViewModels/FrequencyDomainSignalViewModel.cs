using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for display of specific information about <see cref="IFrequencyDomainSignal"/>s
	/// </summary>
	public class FrequencyDomainSignalViewModel : BaseViewModel
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="gainUnit">The unit of values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public FrequencyDomainSignalViewModel(IFrequencyDomainSignal signal)
		{
			if(signal == null)
			{
				throw new ArgumentNullException(nameof(signal));
			}

			Magnitude = signal.Waveform.
				Select((value, counter) => new KeyValuePair<double, double>(
					signal.StartSample + counter * signal.Step,
					20 * Math.Log10(value.Magnitude)));

			Phase = signal.Waveform.
				Select((value, counter) => new KeyValuePair<double, double>(signal.StartSample + counter * signal.Step, value.Phase * 180 / Math.PI));
		}

		#endregion

		#region Public properties
		
		/// <summary>
		/// Magnitude characteristic
		/// </summary>
		public IEnumerable<KeyValuePair<double, double>> Magnitude { get; private set; }

		/// <summary>
		/// Phase characteristic
		/// </summary>
		public IEnumerable<KeyValuePair<double, double>> Phase { get; private set; }

		/// <summary>
		/// Unit for frequency (horizontal axis)
		/// </summary>
		public string XUnit { get; } = "10^(x) " + IoC.Resolve<ISIUnits>().FrequencyShort;

		/// <summary>
		/// Unit for gain values
		/// </summary>
		public string YUnitGain { get; } = IoC.Resolve<ISIUnits>().GainShort;

		/// <summary>
		/// Unit for phase values
		/// </summary>
		public string YUnitPhase { get; } = IoC.Resolve<ISIUnits>().Phase;

		#endregion
	}
}