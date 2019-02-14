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
		/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void AddWaveform(double frequency, IEnumerable<double> instantenousValues) =>
			AddWaveformAndUpdateFinalWaveform(frequency, instantenousValues);

		/// <summary>
		/// Adds a new constant offset to the waveform (it does not overwrite or otherwise invalidate previous offsets)
		/// </summary>
		/// <param name="value"></param>
		public void AddConstantOffset(double value) => AddConstantOffsetAndUpdateFinalWaveform(value);

		#endregion
	}
}