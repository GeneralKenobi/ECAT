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
		/// <param name="unit">The unit of values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public FrequencyDomainSignalViewModel(IFrequencyDomainSignal signal, string unit)
		{
			if(signal == null)
			{
				throw new ArgumentNullException(nameof(signal));
			}

			//Magnitude = signal.Waveform.
			//	Select((value, counter) => new KeyValuePair<double, double>(
			//		signal.StartSample + counter * signal.Step,
			//		20 * Math.Log10(value.Magnitude)));

			Magnitude = signal.Waveform.
				Select((value, counter) => new KeyValuePair<double, double>(
					signal.StartSample + counter * signal.Step,
					value.Magnitude));

			Phase = signal.Waveform.
				Select((value, counter) => new KeyValuePair<double, double>(signal.StartSample + counter * signal.Step, value.Phase));

			YUnit = unit ?? throw new ArgumentNullException(nameof(unit));
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
		public string XUnit { get; } = IoC.Resolve<ISIUnits>().Time;

		/// <summary>
		/// Unit for values
		/// </summary>
		public string YUnit { get; }

		#endregion
	}
}