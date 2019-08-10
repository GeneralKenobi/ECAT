using CSharpEnhanced.CoreInterfaces;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a signal calculated in frequency domain (transfer function charcteristics)
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration]
	[ConstructorDeclaration(typeof(IFrequencyDomainSignal), "Copy constructor")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double)}, "Samples", "Time step", "Start time is equal to 0")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double), typeof(double)}, "Samples", "Time step", "Start time")]
	[ConstructorDeclaration(new Type[] { typeof(IEnumerable<Complex>), typeof(double), typeof(double)}, "Samples", "Time step", "Start time")]
	public interface IFrequencyDomainSignal : IWaveSignal<Complex>, IShallowCopy<IFrequencyDomainSignal>
	{
		#region Methods

		/// <summary>
		/// Creates a copy of the signal in reversed direction (values change their signs)
		/// </summary>
		/// <returns></returns>
		IFrequencyDomainSignal CopyAndNegate();

		#endregion
	}
}