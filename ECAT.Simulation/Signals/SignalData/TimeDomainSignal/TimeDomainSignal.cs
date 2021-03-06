﻿using CSharpEnhanced.CoreInterfaces;
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
	public partial class TimeDomainSignal : ITimeDomainSignal
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
			Waveforms = new ReadOnlyDictionary<ISourceDescription, IEnumerable<double>>(_Waveforms);
		}

		/// <summary>
		/// Constructor with parameters, start time is considered 0
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public TimeDomainSignal(int samples, double timeStep) : this(samples, timeStep, 0) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <param name="startTime">Start time of the signal</param>
		public TimeDomainSignal(int samples, double timeStep, double startTime) : this(samples)
		{
			if(timeStep < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(timeStep));
			}

			TimeStep = timeStep;
			StartTime = startTime;
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
		/// Backing store for <see cref="Waveforms"/>
		/// </summary>
		private IDictionary<ISourceDescription, IEnumerable<double>> _Waveforms { get; } = new Dictionary<ISourceDescription, IEnumerable<double>>();

		/// <summary>
		/// Backing store for <see cref="FinalWaveform"/>
		/// </summary>
		private IList<double> _FinalWaveform { get; } = new List<double>();

		#endregion

		#region Public properties

		/// <summary>
		/// Start time of the simulation, in seconds
		/// </summary>
		public double StartTime { get; private set; }

		/// <summary>
		/// Time elapsed between two calculated values, in seconds
		/// </summary>
		public double TimeStep { get; private set; }

		/// <summary>
		/// Number of samples in this signal
		/// </summary>
		public int Samples { get; private set; }

		/// <summary>
		/// List with instantenous values. Key is the time, value is the signal's value.
		/// </summary>
		public IEnumerable<double> FinalWaveform => _FinalWaveform;

		/// <summary>
		/// All waveforms composing this <see cref="ITimeDomainSignal"/> (AC and DC).
		/// </summary>
		public IReadOnlyDictionary<ISourceDescription, IEnumerable<double>> Waveforms { get; }

		/// <summary>
		/// Object capable of calculating characteristic values for this <see cref="ISignalData"/>
		/// </summary>
		public ISignalDataInterpreter Interpreter { get; }

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
		/// Adds a new waveform to the signal, updates <see cref="FinalWaveform"/>
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
			StartTime = obj.StartTime;
			TimeStep = obj.TimeStep;
			Samples = obj.Samples;

			// Add new data points to _FinalWaveform
			for(int i = 0; i < Samples; ++i)
			{
				_FinalWaveform.Add(0);
			}

			// Copy composing waveforms and constant offsets
			obj.Waveforms.ForEach((x) => AddWaveformHelper(x.Key, x.Value));
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
			var result = new TimeDomainSignal(Samples, TimeStep, StartTime);

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