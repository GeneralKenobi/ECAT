using CSharpEnhanced.CoreInterfaces;
using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a signal calculated in time domain (based on instantenous values calculated for a specific time)
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration(typeof(string), "Unit")]
	[ConstructorDeclaration(typeof(ITimeDomainSignal), "Copy constructor")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double), typeof(string)}, "Samples", "Time step", "Start time is equal to 0", "Unit")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double), typeof(double), typeof(string)}, "Samples", "Time step", "Start time", "Unit")]
	public interface ITimeDomainSignal : IWaveSignal<double>, IShallowCopy<ITimeDomainSignal>
	{
		#region Properties

		/// <summary>
		/// All waveforms composing this <see cref="ITimeDomainSignal"/> (AC and DC).
		/// </summary>
		IReadOnlyDictionary<ISourceDescription, IEnumerable<double>> ComposingWaveforms { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Creates a copy of the signal in reversed direction (values change their signs)
		/// </summary>
		/// <returns></returns>
		ITimeDomainSignal CopyAndNegate();

		#endregion
	}
}