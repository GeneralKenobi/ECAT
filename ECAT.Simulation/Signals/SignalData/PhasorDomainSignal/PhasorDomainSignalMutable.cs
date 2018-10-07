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
		public PhasorDomainSignalMutable() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignalMutable(IPhasorDomainSignal signal) : base(signal) { }

		/// <summary>
		/// Constructor with parameter
		/// </summary>
		/// <param name="dc"></param>
		public PhasorDomainSignalMutable(double dc) : base(dc) { }

		/// <summary>
		/// Constructor with parameter
		/// </summary>
		/// <param name="phasors"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignalMutable(IEnumerable<KeyValuePair<double, Complex>> phasors) : base(phasors) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="phasors">Composing phasors, exception will be thrown if null (use an empty enumeration when there are no phasors)
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignalMutable(double dc, IEnumerable<KeyValuePair<double, Complex>> phasors) : base(dc, phasors) { }		

		#endregion

		#region Public methods

		/// <summary>
		/// Used to set the value of <see cref="INodePotentialBias.DC"/> property
		/// </summary>
		/// <param name="dc"></param>
		public void SetDC(double dc) => DC = dc;

		/// <summary>
		/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="value"></param>
		public void AddPhasor(double frequency, Complex value) => _Phasors.Add(frequency, value);

		#endregion
	}
}