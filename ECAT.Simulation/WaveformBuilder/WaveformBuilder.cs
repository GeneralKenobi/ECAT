using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECAT.Simulation
{
	public static class WaveformBuilder
	{
		#region Public static methods

		/// <summary>
		/// Returns instantenous value of a sine wave
		/// </summary>
		/// <param name="amplitude"></param>
		/// <param name="frequency"></param>
		/// <param name="phaseShift"></param>
		/// <param name="pointIndex"></param>
		/// <param name="argumentStep"></param>
		/// <param name="constantOffset"></param>
		/// <returns></returns>
		public static double SineWaveInstantenousValue(double amplitude, double frequency, double phaseShift, int pointIndex,
			double argumentStep, double constantOffset = 0)
		{
			// Return A * sin(2pi*f*arg + phi) + B
			return amplitude * Math.Sin(2 * Math.PI * frequency * pointIndex * argumentStep + phaseShift) + constantOffset;
		}

		/// <summary>
		/// Builds a sine wave based on given input parameters. X = A * sin(2pi * f * t + phi)
		/// </summary>
		/// <param name="amplitude">Amplitude of the wave (A)</param>
		/// <param name="frequency">Frequency of the wave (f)</param>
		/// <param name="phaseShift">Phase shift of the wave (phi)</param>
		/// <param name="numberOfPoints">Number of points constructed</param>
		/// <param name="argumentStep">Step with which argument (time) is increased with each point</param>
		/// <returns></returns>
		public static IEnumerable<double> SineWave(double amplitude, double frequency, double phaseShift, int numberOfPoints, double argumentStep,
			double constantOffset = 0)
		{
			double argument = 0;

			for (int i = 0; i < numberOfPoints; ++i)
			{
				// Return A * sin(2pi*f*t + phi) + B
				yield return amplitude * Math.Sin(2 * Math.PI * frequency * argument + phaseShift) + constantOffset;
				argument += argumentStep;
			}
		}

		/// <summary>
		/// Returns a constant wave equal to 0.
		/// </summary>
		/// <param name="numberOfPoints">Number of points constructed</param>
		/// <returns></returns>
		public static IEnumerable<double> ZeroWave(int numberOfPoints)
		{
			for (int i = 0; i < numberOfPoints; ++i)
			{
				yield return 0;
			}
		}

		/// <summary>
		/// Shifts the wave by (approximately) phase.
		/// </summary>
		/// <param name="phase">Phase to shift by, in radiansn, has to be greatet than 0 and smaller than 2pi</param>
		public static IEnumerable<double> ShiftWaveform(IEnumerable<double> waveform, double phase)
		{
			// Check if given phase is specified range
			if (phase <= 0 || phase >= 2 * Math.PI)
			{
				throw new ArgumentOutOfRangeException(nameof(phase));
			}

			// Number of samples in the wave
			int pointsCount = waveform.Count();

			// Calculate the splitting point
			int splittingPoint = (int)Math.Round(pointsCount * phase / (2 * Math.PI));

			// Construct the shifted waveform:
			return waveform.
				// Take all points after splitting point
				Skip(splittingPoint).
				Take(pointsCount - splittingPoint).
				// And concat them with all points before splitting point
				Concat(waveform.Take(splittingPoint));
		}

		#endregion
	}
}