using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Manages results of time domain simulations
	/// </summary>
	public partial class SimulationResultsTime : ISimulationResults
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="activeComponentsCurrents"></param>
		/// <param name="timeStep"></param>
		/// <param name="startTime"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public SimulationResultsTime(IEnumerable<KeyValuePair<int, ITimeDomainSignal>> nodes,
			IEnumerable<KeyValuePair<int, ITimeDomainSignal>> activeComponentsCurrents, double timeStep, double startTime)
		{
			var biasVoltage = new TimeVoltage(nodes ?? throw new ArgumentNullException(nameof(nodes)));
			var biasCurrent = new TimeCurrent(
				biasVoltage, activeComponentsCurrents ?? throw new ArgumentNullException(nameof(activeComponentsCurrents)));

			Voltage = biasVoltage;
			Current = biasCurrent;
			Power = new TimePower(biasVoltage, biasCurrent);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Contains information about power, guaranteed to be not null
		/// </summary>
		public IVoltageDB Voltage { get; } 

		/// <summary>
		/// Contains information about power, guaranteed to be not null
		/// </summary>
		public ICurrentDB Current { get; }

		/// <summary>
		/// Contains information about power, guaranteed to be not null
		/// </summary>
		public IPowerDB Power { get; }

		#endregion
	}
}