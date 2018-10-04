using CSharpEnhanced.CoreInterfaces;
using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a signal calculated in time domain (based on instantenous values calculated for a specific time)
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration]
	[ConstructorDeclaration(typeof(ITimeDomainSignal), "Copy constructor")]
	[ConstructorDeclaration(new Type[] { typeof(IEnumerable<double>), typeof(double)}, "Instantenous Values", "Time step", "Start time is equal to 0")]
	[ConstructorDeclaration(new Type[] { typeof(IEnumerable<double>), typeof(double), typeof(double)}, "Instantenous Values", "Time step", "Start time")]
	public interface ITimeDomainSignal : ISignalData, IShallowCopy<ITimeDomainSignal>
	{
		#region Properties

		/// <summary>
		/// Start time of the simulation, in seconds
		/// </summary>
		double StartTime { get; }

		/// <summary>
		/// Time elapsed between two calculated values, in seconds
		/// </summary>
		double TimeStep { get; }		

		/// <summary>
		/// List with calculated instantenous values
		/// </summary>
		IEnumerable<double> InstantenousValues { get; }

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