using System;
using System.Collections.Generic;
using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Factory of <see cref="ITimeDomainSignal"/>s
	/// </summary>
	[RegisterAsInstance(typeof(ITimeDomainSignalFactory))]
	public class TimeDomainSignalFactory : ITimeDomainSignalFactory
	{
		#region Public methods

		/// <summary>
		/// Constructs a <see cref="ITimeDomainSignal"/> equal to 0
		/// </summary>
		/// <returns></returns>
		public ITimeDomainSignal Construct() => new TimeDomainSignal();

		/// <summary>
		/// Constructs a new <see cref="ITimeDomainSignal"/>
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can't be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <param name="startTime">Start time of the signal</param>
		/// <returns></returns
		/// <exception cref="ArgumentNullException"></exception>
		public ITimeDomainSignal Construct(IEnumerable<double> instantenousValues, double timeStep, double startTime = 0) =>
			new TimeDomainSignal(instantenousValues ?? throw new ArgumentNullException(nameof(instantenousValues)), timeStep, startTime);

		/// <summary>
		/// Constructs a shallow copy of <paramref name="source"/>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public ITimeDomainSignal Construct(ITimeDomainSignal source) =>
			new TimeDomainSignal(source ?? throw new ArgumentNullException(nameof(source)));

		/// <summary>
		/// Returns a negation (shallow copy) of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public ITimeDomainSignal ConstructNegation(ITimeDomainSignal signal) =>
			(signal ?? throw new ArgumentNullException(nameof(signal))).CopyAndNegate();

		#endregion
	}
}