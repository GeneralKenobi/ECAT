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
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public GenericPartialStates(int acSourcesCount,
			int dcSourcesCount,
			IEnumerable<int> nodeIndices,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, IActiveComponentDescription, TState> stateFactory = null)
		{
			// If no state factory was provided
			if (stateFactory == null)
			{
				// Create one that simply returns default value for the type
				stateFactory = (x, y, z) => default(TState);
			}

			// Check if number of ac voltage sources descriptions matches the number of ac sources
			if (acVoltageSourcesDescriptions != null && acVoltageSourcesDescriptions.Count() != acSourcesCount)
			{
				throw new ArgumentException(nameof(acVoltageSourcesDescriptions) + $" count does not match {acSourcesCount}");
			}

			// Check if number of ac voltage sources descriptions matches the number of ac sources
			if (dcVoltageSourcesDescriptions != null && dcVoltageSourcesDescriptions.Count() != dcSourcesCount)
			{
				throw new ArgumentException(nameof(acVoltageSourcesDescriptions) + $" count does not match {acSourcesCount}");
			}

			// Create array for AC states
			ACStates = new TState[acSourcesCount];

			// Create array for DC states
			DCStates = new TState[dcSourcesCount];

			// Make a list of ac voltage sources descriptions
			IList<IActiveComponentDescription> acVoltageSourcesDescriptionsList =
				// If the enumeration is null create a sequnce of nulls matching the number of ac voltage sources)
				(acVoltageSourcesDescriptions ?? Enumerable.Repeat<IActiveComponentDescription>(null, acSourcesCount)).ToList();

			// Make a list of dc voltage sources descriptions
			IList<IActiveComponentDescription> dcVoltageSourcesDescriptionsList =
				// If the enumeration is null create a sequnce of nulls matching the number of ac voltage sources)
				(dcVoltageSourcesDescriptions ?? Enumerable.Repeat<IActiveComponentDescription>(null, dcSourcesCount)).ToList();

			// Initialize each entry in the AC array
			for (int i = 0; i < acSourcesCount; ++i)
			{
				ACStates[i] = stateFactory(nodeIndices, activeComponentsIndices, acVoltageSourcesDescriptionsList[i]);
			}

			// Initialize each entry in the DC array
			for (int i = 0; i < acSourcesCount; ++i)
			{
				DCStates[i] = stateFactory(nodeIndices, activeComponentsIndices, dcVoltageSourcesDescriptionsList[i]);
			}

			// Assign the collections to private properties for future use
			_NodeIndices = nodeIndices;
			_ActiveComponentsIndices = activeComponentsIndices;
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public GenericPartialStates(int acSourcesCount,
			int dcSourcesCount,
			IEnumerable<int> nodeIndices, int activeComponentsCount,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, IActiveComponentDescription, TState> stateFactory = null) :
			this(acSourcesCount, dcSourcesCount, nodeIndices, Enumerable.Range(0, activeComponentsCount), acVoltageSourcesDescriptions,
				dcVoltageSourcesDescriptions, stateFactory) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="dcSourcesCount">Number of DC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="stateFactory">Factory method used to generate default values of <see cref="ACStates"/> and <see cref="DCState"/>.
		/// First argument are indices of nodes, second argument are indices of active components currents, third argument is the description
		/// of the source that produced the state</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public GenericPartialStates(int acSourcesCount,
			int dcSourcesCount,
			int nodesCount,
			int activeComponentsCount,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, IActiveComponentDescription, TState> stateFactory = null) :
			this(acSourcesCount, dcSourcesCount, Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount),
				acVoltageSourcesDescriptions, dcVoltageSourcesDescriptions, stateFactory) { }

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
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public GenericPartialStates(int acSourcesCount,
			int dcSourcesCount,
			int nodesCount,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions,
			Func<IEnumerable<int>, IEnumerable<int>, IActiveComponentDescription, TState> stateFactory = null) :
			this(acSourcesCount, dcSourcesCount, Enumerable.Range(0, nodesCount), activeComponentsIndices,acVoltageSourcesDescriptions,
				dcVoltageSourcesDescriptions, stateFactory) { }

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
		public TState[] ACStates { get; }

		/// <summary>
		/// Array holding DC state for each source
		/// </summary>
		public TState[] DCStates { get; }

		#endregion
	}
}