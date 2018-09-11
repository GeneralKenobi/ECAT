using CSharpEnhanced.CoreInterfaces;
using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Signal defined in a phasor domain - composed of a DC offset and some phasors
	/// </summary>
	public class PhasorDomainSignal : IPhasorDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public PhasorDomainSignal() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public PhasorDomainSignal(IPhasorDomainSignal signal)
		{
			Copy(signal);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// DC component of the signal
		/// </summary>
		public double DC { get; set; }

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		public IEnumerable<KeyValuePair<double, Complex>> ComposingPhasors { get; set; }

		/// <summary>
		/// Object capable of calculating characteristic values for this <see cref="ISignalData"/>
		/// </summary>
		public ISignalDataInterpreter Interpreter { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public PhasorDomainSignal Copy() => new PhasorDomainSignal(this);

		/// <summary>
		/// Copies internal state of <paramref name="obj"/> to this instance
		/// </summary>
		/// <param name="obj"></param>
		public void Copy(IPhasorDomainSignal obj)
		{
			DC = obj.DC;
			ComposingPhasors = obj.ComposingPhasors;
		}

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		IPhasorDomainSignal IDeepCopyTo<IPhasorDomainSignal>.Copy() => Copy();
	}

	#endregion
}