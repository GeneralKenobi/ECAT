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
		public PhasorDomainSignalViewModel(IPhasorDomainSignal signal)
		{
			DisplayText = ProcessSignal(signal ?? throw new ArgumentNullException(nameof(signal)));
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
		private IEnumerable<string> ProcessSignal(IPhasorDomainSignal signal)
		{
			// For each phasor in the signal
			foreach(var phasor in signal.Phasors)
			{
				// Start with description of the source
				var infoString = "Source: \"" + phasor.Key.Label.Label + "\" produces ";

				// Then, depending on the category
				switch(phasor.Key.FrequencyCategory)
				{
					case FrequencyCategory.DC:
						{
							infoString += SIHelpers.ToAltSIStringExcludingSmallPrefixes(phasor.Value, signal.Unit, _RoundToDigit) + " DC";
						} break;

					case FrequencyCategory.AC:
						{
							infoString += SIHelpers.ToAltSIStringExcludingSmallPrefixes(phasor.Value, signal.Unit, _RoundToDigit) + " AC";
						}
						break;
				}

				yield return infoString;
			}
		}

		#endregion
	}
}