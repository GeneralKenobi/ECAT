using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Extension of <see cref="TimeDomainSignal"/> (and <see cref="Core.ITimeDomainSignal"/>) that allows for mutating values
	/// </summary>
	[RegisterAsType(typeof(ITimeDomainSignalMutable))]
	public class TimeDomainSignalMutable : TimeDomainSignal, ITimeDomainSignalMutable
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public TimeDomainSignalMutable() { }

		/// <summary>
		/// Constructor with parameters, start time is considered 0
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public TimeDomainSignalMutable(int samples, double timeStep) : base(samples, timeStep) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can'be be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <param name="startTime">Start time of the signal</param>
		public TimeDomainSignalMutable(int samples, double timeStep, double startTime) : base(samples, timeStep, startTime) { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public TimeDomainSignalMutable(ITimeDomainSignal signal) : base(signal) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Adds a new waveform to the signal. If one already exists for source described by <paramref name="description"/>, adds them together,
		/// otherwise makes a new entry in <see cref="ITimeDomainSignal.ACWaveforms"/> or <see cref="ITimeDomainSignal.DCWaveforms"/>
		/// </summary>
		/// <param name="description"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void AddWaveform(ISourceDescription description, IEnumerable<double> values) =>	AddWaveformHelper(description, values);

		/// <summary>
		/// Adds a new waveform to the signal. If one already exists for source described by <paramref name="description"/>, adds them together,
		/// otherwise makes a new entry in <see cref="ITimeDomainSignal.ACWaveforms"/>. or <see cref="ITimeDomainSignal.DCWaveforms"/>.
		/// The waveform is given by a constant value - full
		/// waveform will be constructed from it.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void AddWaveform(ISourceDescription description, double value) =>
			AddWaveform(description, WaveformBuilder.ConstantWaveform(value, Samples));

		#endregion
	}
}