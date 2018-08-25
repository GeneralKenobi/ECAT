using CSharpEnhanced.Helpers;
using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Class containing methods formating text for display in component info section
	/// </summary>
	public static class CIFormat
    {
		#region Private static properties
		
		/// <summary>
		/// Round all values to this digit
		/// </summary>
		private static int _RoundToDigit { get; } = 4;
		
		#endregion

		#region Public static methods

		/// <summary>
		/// Returns a string for display in component info section which will be in the form: 
		/// "{<paramref name="description"/>}: {<paramref name="value"/>}{<paramref name="unit"/>}"
		/// If <paramref name="value"/> is a <see cref="double.NaN"/> it is instead displayed as "unavailable" and the unit is omitted
		/// </summary>
		/// <param name="description"></param>
		/// <param name="value"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		public static string LineInfo(string description, double value, string unit) =>	description + ": " + 
			(double.IsNaN(value) ? "unavailable" : SIHelpers.ToSIStringExcludingSmallPrefixes(value, unit, _RoundToDigit));

		/// <summary>
		/// Returns a string for display in component info section which will be in the form: 
		/// "{<paramref name="description"/>}: {<paramref name="phasor"/>}{<paramref name="unit"/>}"
		/// </summary>
		/// <param name="description"></param>
		/// <param name="phasor"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		public static string LineInfo(string description, Complex phasor, string unit) => description + ": " +
			SIHelpers.ToAltSIStringExcludingSmallPrefixes(phasor, unit, _RoundToDigit);

		/// <summary>
		/// Returns a string for display in component info section which will be in the form: 
		/// "{<paramref name="description"/>}: {<paramref name="phasor"/>}{<paramref name="unit"/>} at {<paramref name="frequency"/>Hz}"
		/// </summary>
		/// <param name="description"></param>
		/// <param name="phasor"></param>
		/// <param name="unit"></param>
		/// <param name="frequency">Frequency of the <paramref name="phasor"/></param>
		/// <returns></returns>
		public static string LineInfo(string description, Complex phasor, string unit, double frequency) =>
			LineInfo(description, phasor, unit) + " at " + 
			SIHelpers.ToAltSIStringExcludingSmallPrefixes(frequency, SIUnits.Singleton.FrequencyShort, _RoundToDigit);

		/// <summary>
		/// Returns info related to some <see cref="ISignalInformation"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="signalName">Name to associate with the signal, eg. voltage</param>
		/// <param name="unit">Unit to use in display</param>
		/// <returns></returns>
		public static IEnumerable<string> GetSignalInfo(ISignalInformation signal, string signalName, string unit)
		{
			// Characteristic signal information
			yield return LineInfo("Minimum instantenous " + signalName, signal.Minimum, unit);
			yield return LineInfo("Maximum instantenous " + signalName, signal.Maximum, unit);
			yield return LineInfo("RMS " + signalName, signal.RMS, unit);

			// DC component information
			if (signal.Type.HasFlag(SignalType.DC))
			{
				yield return LineInfo("DC " + signalName, signal.DC, unit);
			}

			// AC component information
			if (signal.Type.HasFlag(SignalType.AC))
			{
				// If it's a multi-ac signal add a header
				if (signal.Type.HasFlag(SignalType.MultipleAC))
				{
					yield return "Composing AC phasors:";
				}

				// Print each phasor
				foreach (var acWaveform in signal.ComposingPhasors)
				{
					yield return LineInfo("AC " + signalName + " (peak)", acWaveform.Value, unit, acWaveform.Key);
				}
			}
		}

		#endregion
	}
}