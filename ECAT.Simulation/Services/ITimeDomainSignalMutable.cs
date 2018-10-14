using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Extension of <see cref="ITimeDomainSignal"/> that allows for adding/modifying values of public properties of the inherited
	/// interface
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration]
	[ConstructorDeclaration(typeof(ITimeDomainSignal), "Copy constructor")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double) },"Samples", "Instantenous Values", "Time step", "Start time is equal to 0")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double), typeof(double) }, "Samples", "Instantenous Values", "Time step", "Start time")]
	public interface ITimeDomainSignalMutable : ITimeDomainSignal
    {
		#region Methods

		/// <summary>
		/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		void AddWaveform(double frequency, IEnumerable<double> instantenousValues);

		#endregion
	}
}