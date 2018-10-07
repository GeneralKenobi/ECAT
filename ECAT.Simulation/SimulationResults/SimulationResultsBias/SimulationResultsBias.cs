using ECAT.Core;
using System;
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
		/// <param name="nodes"></param>
		/// <param name="activeComponentsCurrents"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public SimulationResultsBias(IEnumerable<KeyValuePair<INode, IPhasorDomainSignal>> nodes,
			IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> activeComponentsCurrents)
		{
			var biasVoltage = new BiasVoltage(nodes ?? throw new ArgumentNullException(nameof(nodes)));
			var biasCurrent = new BiasCurrent(
				biasVoltage, activeComponentsCurrents ?? throw new ArgumentNullException(nameof(activeComponentsCurrents)));

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