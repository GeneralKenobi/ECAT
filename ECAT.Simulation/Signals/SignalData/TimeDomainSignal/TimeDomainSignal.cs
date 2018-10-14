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
	public partial class TimeDomainSignal : ITimeDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public TimeDomainSignal()
		{
			Interpreter = new TimeDomainSignalInterpreter(this);
			ComposingWaveforms = new ReadOnlyDictionary<double, IEnumerable<double>>(_ComposingWaveforms);
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
		public TimeDomainSignal(int samples, double timeStep, double startTime) : this()
		{
			Samples = samples;
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
		/// Backing store for <see cref="ComposingWaveforms"/>
		/// </summary>
		private IDictionary<double, IEnumerable<double>> _ComposingWaveforms { get; } = new Dictionary<double, IEnumerable<double>>();

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
		/// Dictionary of instantenous values of waveforms that compose this signal; key is the frequency of the wave
		/// </summary>
		public IReadOnlyDictionary<double, IEnumerable<double>> ComposingWaveforms { get; }

		/// <summary>
		/// Object capable of calculating characteristic values for this <see cref="ISignalData"/>
		/// </summary>
		public ISignalDataInterpreter Interpreter { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Clears the waveforms in <see cref="_ComposingWaveforms"/>, resets <see cref="_FinalWaveform"/>
		/// </summary>
		private void ClearWaveforms()
		{
			_ComposingWaveforms.Clear();

			for(int i=0; i<Samples; ++i)
			{
				_FinalWaveform[i] = 0;
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Adds a new waveform to the signal, updates <see cref="FinalWaveform"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="values"></param>
		protected void AddWaveform(double frequency, IEnumerable<double> values)
		{
			if(values.Count() != Samples)
			{
				throw new ArgumentException(nameof(values) + $" must have count equal to {Samples} ({nameof(Samples)})");
			}

			// If a waveform for this frequency is already present in this signal
			if(_ComposingWaveforms.ContainsKey(frequency))
			{
				// Add the new values to the entry in the dictionary
				_ComposingWaveforms[frequency] = _ComposingWaveforms[frequency].MergeSelect(values, (x, y) => x + y);
			}
			else
			{
				// Otherwise add a new entry to the dictionary
				_ComposingWaveforms.Add(frequency, values);
			}

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

			StartTime = obj.StartTime;
			TimeStep = obj.TimeStep;
			Samples = obj.Samples;

			ClearWaveforms();
			obj.ComposingWaveforms.ForEach((x) => AddWaveform(x.Key, x.Value));
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
			var result = new TimeDomainSignal(Samples, TimeStep, StartTime);

			ComposingWaveforms.ForEach((x) => result.AddWaveform(x.Key, x.Value));

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