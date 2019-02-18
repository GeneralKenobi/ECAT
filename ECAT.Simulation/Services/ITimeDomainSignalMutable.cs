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
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double) },"Samples", "Time step", "Start time is equal to 0")]
	[ConstructorDeclaration(new Type[] { typeof(int), typeof(double), typeof(double) }, "Samples", "Time step", "Start time")]
	public interface ITimeDomainSignalMutable : ITimeDomainSignal
    {
		#region Methods

		/// <summary>
		/// Adds a new waveform to the signal. If one already exists for source described by <paramref name="description"/>, adds them together,
		/// otherwise makes a new entry in <see cref="ITimeDomainSignal.ACWaveforms"/> or <see cref="ITimeDomainSignal.DCWaveforms"/>
		/// </summary>
		/// <param name="description"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		void AddWaveform(IActiveComponentDescription description, IEnumerable<double> instantenousValues);

		/// <summary>
		/// Adds a new waveform to the signal. If one already exists for source described by <paramref name="description"/>, adds them together,
		/// otherwise makes a new entry in <see cref="ITimeDomainSignal.ACWaveforms"/>. or <see cref="ITimeDomainSignal.DCWaveforms"/>.
		/// The waveform is given by a constant value - full
		/// waveform will be constructed from it.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		void AddWaveform(IActiveComponentDescription description, double value);

		#endregion
	}
}