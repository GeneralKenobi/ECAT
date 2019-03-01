using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

			#region Private methods

			/// <summary>
			/// Returns DC phasors present in <see cref="_Signal"/>
			/// </summary>
			private IEnumerable<double> GetDCPhasors() => _Signal.Phasors.
				Where((x) => x.Key.Frequency == 0).
				Select((x) => x.Value.Real);

			/// <summary>
			/// Returns AC phasors present in <see cref="_Signal"/>
			/// </summary>
			private IEnumerable<Complex> GetACPhasors() => _Signal.Phasors.
				Where((x) => x.Key.Frequency > 0).
				Select((x) => x.Value);

			/// <summary>
			/// Gets all AC phasors, first sums phasors of the same frequency
			/// </summary>
			/// <returns></returns>
			private IEnumerable<Complex> GetACPhasorsWithDifferentFrequencies()
			{
				// Get all AC phasors
				var phasorsList = _Signal.Phasors.Where((x) => x.Key.Frequency > 0).ToList();				

				// While there are AC phasors remaining
				while (phasorsList.Count > 0)
				{
					// Get all phasors with frequency equal to the frequency of the first element in the list
					var singleFrequencyPhasors = phasorsList.FindAll((x) => x.Key.Frequency == phasorsList[0].Key.Frequency);

					// Remove each of those from the list
					singleFrequencyPhasors.ForEach((x) => phasorsList.Remove(x));

					// And return their sum
					yield return singleFrequencyPhasors.Select((x) => x.Value).Sum();
				}
			}

			#endregion

			#region Public methods

			/// <summary>
			/// Calculates and returns the maximum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Maximum() => GetDCPhasors().Sum() + GetACPhasors().Sum((phasor) => phasor.Magnitude);

			/// <summary>
			/// Calculates and returns the minimum instantenous value of the signal
			/// </summary>
			/// <returns></returns>
			public double Minimum() => GetDCPhasors().Sum() - GetACPhasors().Sum((phasor) => phasor.Magnitude);

			/// <summary>
			/// Calculates and returns the root-mean-square value of the signal
			/// </summary>
			/// <returns></returns>
			public double RMS() => Math.Sqrt(Math.Pow(GetDCPhasors().Sum(), 2) +
				// First sum all AC phasors with the same frequency - the formula below is only valid for orthogonal functions.
				// Two sines are orthogonal only if their frequencies are different or if their phase shifts are +- 90 degrees.
				GetACPhasorsWithDifferentFrequencies().Sum((x) => Math.Pow(x.Magnitude / Math.Sqrt(2), 2)));

			/// <summary>
			/// Returns the average value of the signal
			/// </summary>
			/// <returns></returns>
			public double Average() => GetDCPhasors().Sum();
			// Average value is equal to DC (sine waves have average value equal to 0)

			#endregion
		}
	}
}