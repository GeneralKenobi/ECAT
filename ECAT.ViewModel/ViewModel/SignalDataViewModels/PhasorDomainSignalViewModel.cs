using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for display of specific information about <see cref="IPhasorDomainSignal"/>s
	/// </summary>
	public class PhasorDomainSignalViewModel : BaseViewModel
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="signal"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignalViewModel(IPhasorDomainSignal signal, string unit)
		{
			DisplayText = ProcessSignal(
				signal ?? throw new ArgumentNullException(nameof(signal)),
				unit ?? throw new ArgumentNullException(nameof(unit)));
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Digit to round the value to
		/// </summary>
		private int _RoundToDigit { get; } = 4;

		#endregion

		#region Public properties

		/// <summary>
		/// Text ready to be displayed on screen, contains detailed information about the <see cref="IPhasorDomainSignal"/>
		/// </summary>
		public IEnumerable<string> DisplayText { get; private set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Processes signal and returns a sequence of strings to print on screen
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		private IEnumerable<string> ProcessSignal(IPhasorDomainSignal signal, string unit)
		{
			// If DC is not 0, print it
			if(signal.DC != 0)
			{
				yield return "DC: " + SIHelpers.ToSIStringExcludingSmallPrefixes(signal.DC, unit, _RoundToDigit);
			}
			
			// Print each phasor
			foreach (var acWaveform in signal.Phasors)
			{
				yield return "Phasor (peak): " + acWaveform.Value + unit + " at " + acWaveform.Key +
					IoC.Resolve<ISIUnits>().FrequencyShort;
			}			
		}

		#endregion
	}
}