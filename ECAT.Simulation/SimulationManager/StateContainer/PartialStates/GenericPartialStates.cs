using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Generic container for partial states - <see cref="GenericState{T}"/>.
	/// </summary>
	/// <typeparam name="TState">Specific type of <see cref="GenericState{T}"/></typeparam>
	/// <typeparam name="TValues">Type of values used in <typeparamref name="TState"/></typeparam>
	public class GenericPartialStates<TState, TValues> where TState : GenericState<TValues>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public GenericPartialStates(IEnumerable<int> nodeIndices,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<ISourceDescription> sourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, ISourceDescription, TState> stateFactory = null)
		{
			// If no state factory was provided
			if (stateFactory == null)
			{
				// Create one that simply returns default value for the type
				stateFactory = (x, y, z) => default(TState);
			}

			if(sourcesDescriptions == null)
			{
				throw new ArgumentNullException(nameof(sourcesDescriptions));
			}

			// Assign the collections to private properties for future use
			_NodeIndices = nodeIndices;
			_ActiveComponentsIndices = activeComponentsIndices;
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public GenericPartialStates(IEnumerable<int> nodeIndices, int activeComponentsCount,
			IEnumerable<ISourceDescription> sourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, ISourceDescription, TState> stateFactory = null) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), sourcesDescriptions, stateFactory) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public GenericPartialStates(int nodesCount,
			int activeComponentsCount,
			IEnumerable<ISourceDescription> sourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, ISourceDescription, TState> stateFactory = null) :
			this(Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount), sourcesDescriptions, stateFactory) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="sourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		public GenericPartialStates(int nodesCount,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<ISourceDescription> sourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, ISourceDescription, TState> stateFactory = null) :
			this(Enumerable.Range(0, nodesCount), activeComponentsIndices, sourcesDescriptions, stateFactory) { }

		#endregion

		#region Protected properties

		/// <summary>
		/// Indices of nodes present in this instance
		/// </summary>
		protected IEnumerable<int> _NodeIndices { get; }

		/// <summary>
		/// Active components indices present in this instance
		/// </summary>
		protected IEnumerable<int> _ActiveComponentsIndices { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// Array holding AC state for each source
		/// </summary>
		public IDictionary<ISourceDescription, TState> States { get; } = new Dictionary<ISourceDescription, TState>();

		#endregion
	}
}