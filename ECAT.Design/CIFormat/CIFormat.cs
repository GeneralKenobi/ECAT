using CSharpEnhanced.Helpers;
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

		/// <summary>
		/// Unit of frequency
		/// </summary>
		private static string _FrequencyUnit { get; } = "Hz";
		
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
			SIHelpers.ToAltSIStringExcludingSmallPrefixes(frequency, _FrequencyUnit, _RoundToDigit);

		#endregion
	}
}