using CSharpEnhanced.CoreInterfaces;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing signal composed of a DC offset and phasors
	/// </summary>
	public interface IPhasorDomainSignal : ISignalData, IDeepCopy<IPhasorDomainSignal>
	{
		#region Properties

		/// <summary>
		/// DC component of the signal
		/// </summary>
		double DC { get; }

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		IEnumerable<KeyValuePair<double, Complex>> ComposingPhasors { get; }

		/// <summary>
		/// The type of the signal
		/// </summary>
		SignalType Type { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Creates a copy of the signal in reversed direction (<see cref="DC"/> and each <see cref="ComposingPhasors"/> value is negated)
		/// </summary>
		/// <returns></returns>
		IPhasorDomainSignal CopyAndNegate();

		#endregion
	}
}