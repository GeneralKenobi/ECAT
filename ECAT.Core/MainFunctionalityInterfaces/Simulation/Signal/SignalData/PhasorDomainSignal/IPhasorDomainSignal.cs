using CSharpEnhanced.CoreInterfaces;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing signal composed of a DC offset and phasors
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration]
	[ConstructorDeclaration(typeof(IEnumerable<KeyValuePair<double, Complex>>), "Phasors")]
	[ConstructorDeclaration(typeof(IPhasorDomainSignal), "Copy constructor")]
	public interface IPhasorDomainSignal : ISignalData, IShallowCopy<IPhasorDomainSignal>
	{
		#region Properties

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		IDictionary<ISourceDescription, Complex> Phasors { get; }

		/// <summary>
		/// The type of the signal
		/// </summary>
		SignalType Type { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Creates a copy of the signal in reversed direction (<see cref="DC"/> and each <see cref="Phasors"/> value is negated)
		/// </summary>
		/// <returns></returns>
		IPhasorDomainSignal CopyAndNegate();

		#endregion
	}
}