using System;
using System.Collections.Generic;
using System.Numerics;
using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Factory of <see cref="PhasorDomainSignal"/>s
	/// </summary>
	[RegisterAsInstance(typeof(IPhasorDomainSignalFactory))]
	public class PhasorDomainSignalFactory : IPhasorDomainSignalFactory
	{
		#region Public methods

		/// <summary>
		/// Constructs a <see cref="IPhasorDomainSignal"/> equal to 0
		/// </summary>
		/// <returns></returns>
		public IPhasorDomainSignal Construct() => new PhasorDomainSignal();

		/// <summary>
		/// Constructs an <see cref="IPhasorDomainSignal"/> with only a DC offset
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public IPhasorDomainSignal Construct(double dc) => new PhasorDomainSignal(dc);

		/// <summary>
		/// Constructs an <see cref="IPhasorDomainSignal"/> out of an enumeration of phasors and with DC offset equal to 0
		/// </summary>
		/// <param name="phasors"></param>
		/// <returns></returns>
		public IPhasorDomainSignal Construct(IEnumerable<KeyValuePair<double, Complex>> phasors) =>
			new PhasorDomainSignal(phasors ?? throw new ArgumentNullException(nameof(phasors)));

		/// <summary>
		/// Constructs an <see cref="IPhasorDomainSignal"/> with DC offset and an enumeration of phasors
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="phasors"></param>
		/// <returns></returns>
		public IPhasorDomainSignal Construct(double dc, IEnumerable<KeyValuePair<double, Complex>> phasors) =>
			new PhasorDomainSignal(dc, phasors ?? throw new ArgumentNullException(nameof(phasors)));

		/// <summary>
		/// Constructs a shallow copy of <paramref name="source"/>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public IPhasorDomainSignal Construct(IPhasorDomainSignal source) => source.Copy();

		/// <summary>
		/// Returns a negation (shallow copy) of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		public IPhasorDomainSignal ConstructNegation(IPhasorDomainSignal signal) => signal.CopyAndNegate();

		#endregion
	}
}