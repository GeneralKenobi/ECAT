using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Generic class for interpreting some signal information
		/// </summary>
		private class GenericSignalInformationInterpreter : ISignalInformationInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="signalName">The name of the interpreted signal (eg. "Voltage")</param>
			/// <param name="unit">The unit of the signal's value (eg. "V" or "Volts")</param>
			/// <param name="roundToDigit">All values are rounded to this digit (counting from first leading non-zero digit or
			/// zero standing in front of decimal point, whichever comes first)</param>
			public GenericSignalInformationInterpreter(string signalName, string unit, int roundToDigit = 4)
			{
				_SignalName = signalName ?? throw new ArgumentNullException(nameof(signalName));
				_Unit = unit ?? throw new ArgumentNullException(nameof(unit));
				_RoundToDigit = roundToDigit;
			}

			#endregion

			#region Private properties

			/// <summary>
			/// All values are rounded to this digit (counting from first leading non-zero digit or zero standing in front of decimal
			/// point, whichever comes first)
			/// </summary>
			private int _RoundToDigit { get; }

			/// <summary>
			/// The name of the interpreted signal (eg. "Voltage")
			/// </summary>
			private string _SignalName { get; }

			/// <summary>
			/// The unit of the signal's value (eg. "V" or "Volts")
			/// </summary>
			private string _Unit { get; }

			#endregion

			#region Private methods

			/// <summary>
			/// Returns a string for display in component info section which will be in the form: 
			/// "{<paramref name="description"/>}: {<paramref name="value"/>}{<paramref name="unit"/>}"
			/// If <paramref name="value"/> is a <see cref="double.NaN"/> it is instead displayed as "unavailable" and the unit is omitted
			/// </summary>
			/// <param name="description"></param>
			/// <param name="value"></param>
			/// <param name="unit"></param>
			/// <returns></returns>
			private string LineInfo(string description, double value, string unit) => description + ": " +
				(double.IsNaN(value) ? "unavailable" : SIHelpers.ToSIStringExcludingSmallPrefixes(value, unit, _RoundToDigit));

			#endregion

			#region Public methods

			/// <summary>
			/// Interprets the given signal and returns a sequence of strings ready to be displayed on the screen
			/// </summary>
			/// <param name="info"></param>
			/// <returns></returns>
			public IEnumerable<string> Get(ISignalInformation info)
			{
				// Return empty sequence for null info
				if (info == null)
				{
					yield break;
				}

				// Characteristic signal information
				yield return LineInfo("Minimum instantenous " + _SignalName, info.Minimum, _Unit);
				yield return LineInfo("Maximum instantenous " + _SignalName, info.Maximum, _Unit);
				yield return LineInfo("RMS " + _SignalName, info.RMS, _Unit);
				yield return LineInfo("Average " + _SignalName, info.Average, _Unit);
			}

			#endregion
		}
	}
}