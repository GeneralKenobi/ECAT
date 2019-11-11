using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Extension of <see cref="PhasorDomainSignal"/> (and <see cref="Core.IPhasorDomainSignal"/>) that allows for mutating values
	/// </summary>
	[RegisterAsType(typeof(IPhasorDomainSignalMutable))]
	public class PhasorDomainSignalMutable : PhasorDomainSignal, IPhasorDomainSignalMutable
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public PhasorDomainSignalMutable(string unit) : base(unit){ }

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignalMutable(IPhasorDomainSignal signal) : base(signal) { }

		/// <summary>
		/// Constructor with parameter
		/// </summary>
		/// <param name="phasors"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignalMutable(IEnumerable<KeyValuePair<ISourceDescription, Complex>> phasors, string unit) : base(phasors, unit) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
		/// </summary>
		/// <param name="source"></param>
		/// <param name="value"></param>
		public void AddPhasor(ISourceDescription source, Complex value) => AddPhasorHelper(source, value);

		#endregion
	}
}