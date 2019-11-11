using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Signal defined by a series of points with instantenous values for a specific time
	/// </summary>
	public abstract class WaveSignal<T> : IWaveSignal<T>
	{
		#region Constructors

		/// <summary>
		/// Constructor which initializes <see cref="_FinalWaveform"/>
		/// </summary>
		/// <param name="samples"></param>
		protected WaveSignal(int samples, T initialSamplesValue, string unit) : this(unit)
		{
			// Check if value is correct
			if (samples < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(samples));
			}

			Samples = samples;

			// Add samples to _FinalWaveform
			for (int i = 0; i < samples; ++i)
			{
				_FinalWaveform.Add(initialSamplesValue);
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		protected WaveSignal(string unit)
		{
			Unit = unit;
		}

		/// <summary>
		/// Constructor with parameters, start time is considered 0
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <exception cref="ArgumentNullException"></exception>
		protected WaveSignal(int samples, double timeStep, T initialSamplesValue, string unit) : this(samples, timeStep, 0, initialSamplesValue, unit) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <param name="startTime">Start time of the signal</param>
		protected WaveSignal(int samples, double timeStep, double startTime, T initialSamplesValue, string unit) : this(samples, initialSamplesValue, unit)
		{
			if (timeStep < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(timeStep));
			}

			Step = timeStep;
			StartSample = startTime;
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Backing store for <see cref="Waveform"/>
		/// </summary>
		protected IList<T> _FinalWaveform { get; } = new List<T>();

		#endregion

		#region Public properties
		
		/// <summary>
		/// Unit to dislay
		/// </summary>
		public string Unit { get; }

		/// <summary>
		/// Start time of the simulation, in seconds
		/// </summary>
		public double StartSample { get; protected set; }

		/// <summary>
		/// Time elapsed between two calculated values, in seconds
		/// </summary>
		public double Step { get; protected set; }

		/// <summary>
		/// Number of samples in this signal
		/// </summary>
		public int Samples { get; protected set; }

		/// <summary>
		/// List with instantenous values. Key is the time, value is the signal's value.
		/// </summary>
		public IEnumerable<T> Waveform => _FinalWaveform;

		/// <summary>
		/// Interpreter for this signal data
		/// </summary>
		public ISignalDataInterpreter Interpreter { get; protected set; }

		#endregion
	}
}
