using CSharpEnhanced.CoreInterfaces;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Signal defined by a series of points with instantenous values for a specific time
	/// </summary>
	[RegisterAsType(typeof(IFrequencyDomainSignal))]
	public partial class FrequencyDomainSignal : WaveSignal<Complex>, IFrequencyDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Constructor which initializes <see cref="_FinalWaveform"/>
		/// </summary>
		/// <param name="samples"></param>
		private FrequencyDomainSignal(int samples) : this()
		{
			// Check if value is correct
			if(samples < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(samples));
			}

			Samples = samples;

			// Add samples to _FinalWaveform
			for (int i = 0; i < samples; ++i)
			{
				_FinalWaveform.Add(0);
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public FrequencyDomainSignal() : base("")
		{
			Interpreter = new FrequencyDomainSignalInterpreter(this);
		}

		/// <summary>
		/// Constructor with parameters, start time is considered 0
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="step">Time step between two subsequent values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public FrequencyDomainSignal(int samples, double step) : this(samples, step, 0) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="step">Time step between two subsequent values</param>
		/// <param name="startSample">Start time of the signal</param>
		public FrequencyDomainSignal(int samples, double step, double startSample) : base(samples, step, startSample, 0, "")
		{
			if(step < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(step));
			}

			Step = step;
			StartSample = startSample;
			Interpreter = new FrequencyDomainSignalInterpreter(this);
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="step">Time step between two subsequent values</param>
		/// <param name="startSample">Start time of the signal</param>
		public FrequencyDomainSignal(IEnumerable<Complex> samples, double step, double startSample) : base(samples.Count(), step, startSample, 0, "")
		{
			if (step < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(step));
			}

			Step = step;
			StartSample = startSample;
			samples.ForEach((x, i) => _FinalWaveform[i]+=(x));
			Interpreter = new FrequencyDomainSignalInterpreter(this);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public FrequencyDomainSignal(IFrequencyDomainSignal signal) : this()
		{
			Copy(signal ?? throw new ArgumentNullException(nameof(signal)));
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Second unit for phase
		/// </summary>
		public string SecondUnit { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public FrequencyDomainSignal CopySignal() => new FrequencyDomainSignal(this);

		/// <summary>
		/// Copies internal state of <paramref name="obj"/> to this instance
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Copy(IFrequencyDomainSignal obj)
		{
			if(obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			// Copy properties
			StartSample = obj.StartSample;
			Step = obj.Step;
			Samples = obj.Samples;

			// Add new data points to _FinalWaveform
			obj.Waveform.ForEach((x, i) => _FinalWaveform[i] = x);
		}

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		IFrequencyDomainSignal IShallowCopyTo<IFrequencyDomainSignal>.Copy() => CopySignal();

		/// <summary>
		/// Creates a copy of the signal in reversed direction (values change their signs)
		/// </summary>
		/// <returns></returns>
		public FrequencyDomainSignal CopyAndNegate() =>
			// Create result based on internal properties
			new FrequencyDomainSignal(_FinalWaveform.Select((x) => -x), Step, StartSample);

		/// <summary>
		/// Creates a copy of the signal in reversed direction (values change their signs)
		/// </summary>
		/// <returns></returns>
		IFrequencyDomainSignal IFrequencyDomainSignal.CopyAndNegate() => CopyAndNegate();

		#endregion
	}
}