﻿using ECAT.Core;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Standard implementation of <see cref="ISimulationResults"/>, manages results for <see cref="SimulationManager"/>
	/// </summary>
	public partial class SimulationResultsBias : ISimulationResults
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public SimulationResultsBias(IEnumerable<INode> nodes, IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> activeComponentsCurrents)
		{
			var biasVoltage = new BiasVoltage(nodes);
			var biasCurrent = new BiasCurrent(biasVoltage, activeComponentsCurrents);

			Voltage = biasVoltage;
			Current = biasCurrent;
			Power = new BiasPower(biasVoltage, biasCurrent);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Contains information about power
		/// </summary>
		public IVoltageDB Voltage { get; }

		/// <summary>
		/// Contains information about power
		/// </summary>
		public ICurrentDB Current { get; }

		/// <summary>
		/// Contains information about power
		/// </summary>
		public IPowerDB Power { get; }

		#endregion
	}
}