using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Manages results of time domain simulations
	/// </summary>
	public partial class SimulationResultsFrequency : ISimulationResults
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
		public SimulationResultsFrequency(IEnumerable<KeyValuePair<int, IFrequencyDomainSignal>> nodes)
		{
			Voltage = new FrequencyVoltage(nodes ?? throw new ArgumentNullException(nameof(nodes)));
			Current = new SimulationResultsProvider.DummyCurrentDB();
			Power = new SimulationResultsProvider.DummyPowerDB();
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