﻿using CSharpEnhanced.CoreInterfaces;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a signal calculated in time domain (based on instantenous values calculated for a specific time)
	/// </summary>
	public interface ITimeDomainSignal : ISignalData, IDeepCopy<ITimeDomainSignal>
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
	}
}