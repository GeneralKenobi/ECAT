using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for instantenous state of a system
	/// </summary>
	public class InstantenousPartialStates
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		public InstantenousPartialStates(int acSourcesCount, IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices)
		{
			// Create container for DC state
			DCState = new InstantenousState(nodeIndices, activeComponentsIndices);

			// Create array for AC states
			ACStates = new InstantenousState[acSourcesCount];

			// And initialize each entry in the array
			for(int i = 0; i < acSourcesCount; ++i)
			{
				ACStates[i] = new InstantenousState(nodeIndices, activeComponentsIndices);
			}

			// Assign the collections to private properties for future use
			_NodeIndices = nodeIndices;
			_ActiveComponentsIndices = activeComponentsIndices;
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		public InstantenousPartialStates(int acSourcesCount, IEnumerable<int> nodeIndices, int activeComponentsCount) :
			this(acSourcesCount, nodeIndices, Enumerable.Range(0, activeComponentsCount)) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		public InstantenousPartialStates(int acSourcesCount, int nodesCount, int activeComponentsCount) :
			this(acSourcesCount, Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount)) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		public InstantenousPartialStates(int acSourcesCount, int nodesCount, IEnumerable<int> activeComponentsIndices) :
			this(acSourcesCount, Enumerable.Range(0, nodesCount), activeComponentsIndices) { }

		#endregion

		#region Private properties

		/// <summary>
		/// Indices of nodes present in this instance
		/// </summary>
		private IEnumerable<int> _NodeIndices { get; }

		/// <summary>
		/// Active components indices present in this instance
		/// </summary>
		private IEnumerable<int> _ActiveComponentsIndices { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// Array holding AC instantenous potentials for each source
		/// </summary>
		public InstantenousState[] ACStates { get; }

		/// <summary>
		/// DC instantenous state of the system
		/// </summary>
		public InstantenousState DCState { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Combines (adds) all states (for every AC source and DC) and returns it
		/// </summary>
		/// <returns></returns>
		public InstantenousState Combine()
		{
			// Create result
			var result = new InstantenousState(_NodeIndices, _ActiveComponentsIndices);

			// Add every AC state
			for(int i = 0; i < ACStates.Length; ++i)
			{
				result.AddState(ACStates[i]);
			}

			// Add DC state
			result.AddState(DCState);

			return result;
		}

		#endregion
	}
}