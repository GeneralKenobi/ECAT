using ECAT.Core;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for instantenous state of a system
	/// </summary>
	public class InstantenousPartialStates : GenericPartialStates<InstantenousState, double>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public InstantenousPartialStates(int acSourcesCount,
			int dcSourcesCount,
			IEnumerable<int> nodeIndices,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			base(acSourcesCount, dcSourcesCount, nodeIndices, activeComponentsIndices, acVoltageSourcesDescriptions, dcVoltageSourcesDescriptions,
				(x, y, z) => new InstantenousState(x, y, z)) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public InstantenousPartialStates(int acSourcesCount,
			int dcSourcesCount,
			IEnumerable<int> nodeIndices,
			int activeComponentsCount,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			this(acSourcesCount, dcSourcesCount, nodeIndices, Enumerable.Range(0, activeComponentsCount), acVoltageSourcesDescriptions,
				dcVoltageSourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public InstantenousPartialStates(int acSourcesCount,
			int dcSourcesCount,
			int nodesCount,
			int activeComponentsCount,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			this(acSourcesCount, dcSourcesCount, Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount),
				acVoltageSourcesDescriptions, dcVoltageSourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public InstantenousPartialStates(int acSourcesCount,
			int dcSourcesCount,
			int nodesCount,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			this(acSourcesCount, dcSourcesCount, Enumerable.Range(0, nodesCount), activeComponentsIndices, acVoltageSourcesDescriptions,
				dcVoltageSourcesDescriptions) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Combines (adds) all states (for every AC source and DC) and returns it
		/// </summary>
		/// <returns></returns>
		public InstantenousState Combine()
		{
			// Create result
			var result = new InstantenousState(_NodeIndices, _ActiveComponentsIndices, null);

			// Add every AC state
			for(int i = 0; i < ACStates.Length; ++i)
			{
				result.AddState(ACStates[i]);
			}

			// Add DC states
			for (int i = 0; i < DCStates.Length; ++i)
			{
				result.AddState(DCStates[i]);
			}

			return result;
		}

		#endregion
	}
}