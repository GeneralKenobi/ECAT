using CSharpEnhanced.CoreInterfaces;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Signal defined by a series of points with instantenous values for a specific time
	/// </summary>
	[RegisterAsType(typeof(ITimeDomainSignal))]
	public partial class TimeDomainSignal : WaveSignal<double>, ITimeDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Constructor which initializes <see cref="_FinalWaveform"/>
		/// </summary>
		/// <param name="samples"></param>
		private TimeDomainSignal(int samples) : this()
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
		public TimeDomainSignal()
		{
			Interpreter = new TimeDomainSignalInterpreter(this);
			ComposingWaveforms = new ReadOnlyDictionary<ISourceDescription, IEnumerable<double>>(_Waveforms);
		}

		/// <summary>
		/// Constructor with parameters, start time is considered 0
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="step">Time step between two subsequent values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public TimeDomainSignal(int samples, double step) : this(samples, step, 0) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="step">Time step between two subsequent values</param>
		/// <param name="startSample">Start time of the signal</param>
		public TimeDomainSignal(int samples, double step, double startSample) : base(samples, step, startSample)
		{
			if(step < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(step));
			}

			Step = step;
			StartSample = startSample;
			Interpreter = new TimeDomainSignalInterpreter(this);
			ComposingWaveforms = new ReadOnlyDictionary<ISourceDescription, IEnumerable<double>>(_Waveforms);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public TimeDomainSignal(ITimeDomainSignal signal) : this()
		{
			Copy(signal ?? throw new ArgumentNullException(nameof(signal)));
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="ComposingWaveforms"/>
		/// </summary>
		private IDictionary<ISourceDescription, IEnumerable<double>> _Waveforms { get; } = new Dictionary<ISourceDescription, IEnumerable<double>>();

		#endregion

		#region Public properties

		/// <summary>
		/// All waveforms composing this <see cref="ITimeDomainSignal"/> (AC and DC).
		/// </summary>
		public IReadOnlyDictionary<ISourceDescription, IEnumerable<double>> ComposingWaveforms { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Clears composing waveforms, constant offsets and resets <see cref="_FinalWaveform"/>
		/// </summary>
		private void Clear()
		{
			_Waveforms.Clear();

			for(int i = 0; i < Samples; ++i)
			{
				_FinalWaveform[i] = 0;
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Adds a new waveform to the signal, updates <see cref="Waveform"/>
		/// </summary>
		/// <param name="description">Positive value</param>
		/// <param name="values"></param>
		protected void AddWaveformHelper(ISourceDescription description, IEnumerable<double> values)
		{
			// Null check
			if(description == null || values == null)
			{
				throw new ArgumentNullException();
			}

			// Check if number of samples in the waveform matches this time domain signal
			if(values.Count() != Samples)
			{
				throw new ArgumentException(nameof(values) + $" must have count equal to {Samples} ({nameof(Samples)})");
			}

			// If the source description is already present in the collection
			if(_Waveforms.ContainsKey(description))
			{
				// Add the new values to those already present
				_Waveforms[description] = _Waveforms[description].MergeSelect(values, (x, y) => x + y);
			}
			else
			{
				// Otherwise make a new entry
				_Waveforms.Add(description, values);
			}

			// And finally add values to the final waveform
			values.ForEach((x, i) => _FinalWaveform[i] += x);
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public TimeDomainSignal CopySignal() => new TimeDomainSignal(this);

		/// <summary>
		/// Copies internal state of <paramref name="obj"/> to this instance
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Copy(ITimeDomainSignal obj)
		{
			if(obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			// Clear old waveforms
			Clear();

			// Copy properties
			StartSample = obj.StartSample;
			Step = obj.Step;
			Samples = obj.Samples;

			// Add new data points to _FinalWaveform
			for(int i = 0; i < Samples; ++i)
			{
				_FinalWaveform.Add(0);
			}

			// Copy composing waveforms and constant offsets
			obj.ComposingWaveforms.ForEach((x) => AddWaveformHelper(x.Key, x.Value));
		}

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		ITimeDomainSignal IShallowCopyTo<ITimeDomainSignal>.Copy() => CopySignal();

		/// <summary>
		/// Creates a copy of the signal in reversed direction (values change their signs)
		/// </summary>
		/// <returns></returns>
		public TimeDomainSignal CopyAndNegate()
		{
			// Create result based on internal properties
			var result = new TimeDomainSignal(Samples, Step, StartSample);

			// Copy waveforms and constant offsets with switched sign
			_Waveforms.ForEach((x) => result.AddWaveformHelper(x.Key, x.Value.Select((y) => -y)));

			return result;
		}

		/// <summary>
		/// Creates a copy of the signal in reversed direction (values change their signs)
		/// </summary>
		/// <returns></returns>
		ITimeDomainSignal ITimeDomainSignal.CopyAndNegate() => CopyAndNegate();

		#endregion
	}
}