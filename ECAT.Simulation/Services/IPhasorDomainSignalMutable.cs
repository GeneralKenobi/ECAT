using ECAT.Core;
using System;
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
	[ConstructorDeclaration(typeof(double), "DC")]
	[ConstructorDeclaration(typeof(IEnumerable<KeyValuePair<double, Complex>>), "Phasors")]
	[ConstructorDeclaration(new Type[] { typeof(double), typeof(IEnumerable<KeyValuePair<double, Complex>>) }, "DC", "Phasors")]
	[ConstructorDeclaration(typeof(IPhasorDomainSignal), "Copy constructor")]
	public interface IPhasorDomainSignalMutable : IPhasorDomainSignal
    {
		#region Methods

		/// <summary>
		/// Used to set the value of <see cref="INodePotentialBias.DC"/> property
		/// </summary>
		/// <param name="dc"></param>
		void SetDC(double dc);

		/// <summary>
		/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="value"></param>
		void AddPhasor(double frequency, Complex value);

		#endregion
	}
}