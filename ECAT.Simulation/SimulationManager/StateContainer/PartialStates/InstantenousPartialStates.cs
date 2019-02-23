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
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public InstantenousPartialStates(IEnumerable<int> nodeIndices,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			base(nodeIndices, activeComponentsIndices, sourcesDescriptions, (x, y, z) => new InstantenousState(x, y, z)) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public InstantenousPartialStates(IEnumerable<int> nodeIndices,
			int activeComponentsCount,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), sourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public InstantenousPartialStates(int nodesCount,
			int activeComponentsCount,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			this(Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount), sourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public InstantenousPartialStates(int nodesCount,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			this(Enumerable.Range(0, nodesCount), activeComponentsIndices, sourcesDescriptions) { }

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

			// Add every state
			foreach(var state in States.Values)
			{
				result.AddState(state);
			}

			return result;
		}

		#endregion
	}
}