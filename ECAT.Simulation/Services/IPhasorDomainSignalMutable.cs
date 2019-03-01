using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Extension of <see cref="IPhasorDomainSignal"/> that allows for adding/modifying values of public properties of the inherited
	/// interface
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration]
	[ConstructorDeclaration(typeof(IEnumerable<KeyValuePair<ISourceDescription, Complex>>), "Phasors")]
	[ConstructorDeclaration(typeof(IPhasorDomainSignal), "Copy constructor")]
	public interface IPhasorDomainSignalMutable : IPhasorDomainSignal
    {
		#region Methods

		/// <summary>
		/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
		/// </summary>
		/// <param name="source"></param>
		/// <param name="value"></param>
		void AddPhasor(ISourceDescription source, Complex value);

		#endregion
	}
}