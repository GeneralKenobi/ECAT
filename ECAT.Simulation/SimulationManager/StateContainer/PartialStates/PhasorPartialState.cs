using ECAT.Core;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for instantenous state of a system
	/// </summary>
	public class PhasorPartialStates : GenericPartialStates<PhasorState, Complex>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public PhasorPartialStates(IEnumerable<int> nodeIndices,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			base(nodeIndices, activeComponentsIndices, sourcesDescriptions,	(x, y, z) => new PhasorState(x, y, z)) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public PhasorPartialStates(IEnumerable<int> nodeIndices,
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
		/// <param name="sourcesDescriptions">Descriptions of sources that will produce the partial states</param>
		public PhasorPartialStates(int nodesCount,
			int activeComponentsCount,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			this(Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount), sourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		public PhasorPartialStates(int nodesCount,
			IEnumerable<int> activeComponentsIndices,
			IEnumerable<ISourceDescription> sourcesDescriptions) :
			this(Enumerable.Range(0, nodesCount), activeComponentsIndices, sourcesDescriptions) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Merges <paramref name="other"/> into this instance. Adds up all states that are present in both <see cref="PhasorPartialStates"/> and
		/// makes new entries for all states present in <paramref name="other"/> but not in this instance.
		/// </summary>
		/// <param name="other"></param>
		public void MergeWith(PhasorPartialStates other)
		{
			// For all keys in other that are present in this instance
			foreach(var key in States.Keys.Intersect(other.States.Keys))
			{
				// Add corresponding states from other to already existing states in this instance
				States[key].AddState(other.States[key]);
			}

			// For all keys in other that are not present in this instance
			foreach(var key in other.States.Keys.Except(States.Keys))
			{
				// Add them, with values corresponding to them, to this instance
				States.Add(key, other.States[key]);
			}
		}

		/// <summary>
		/// Converts every state to its DC version (taking only real part from phasor - phasors for DC are purely real). If any of the sources is not,
		/// DC an exception will be thrown.
		/// </summary>
		/// <returns></returns>
		public InstantenousPartialStates ToDC()
		{
			// Create state for result
			var result = new InstantenousPartialStates(_NodeIndices, _ActiveComponentsIndices, States.Keys);

			// For each state
			foreach (var state in States)
			{
				// Use its method to convert to DC
				result.States[state.Key] = States[state.Key].ToDC();
			}

			return result;
		}

		/// <summary>
		/// Creates an instantenous state from each state. The time moment is defined as <paramref name="pointIndex"/> * <paramref name="timeStep"/>.
		/// </summary>
		/// <returns></returns>
		/// <param name="pointIndex">Index of the point for which the instantenous values will be calculated, indexing starts from 0</param>
		/// <param name="timeStep">Time step between 2 subsequent points</param>
		public InstantenousPartialStates ToInstantenousValue(int pointIndex, double timeStep)
		{
			// Create state for result
			var result = new InstantenousPartialStates(_NodeIndices, _ActiveComponentsIndices, States.Keys);

			// For each state
			foreach (var state in States)
			{
				// Use its method to calculate instantenous value at given time moment
				result.States[state.Key] = States[state.Key].ToInstantenousValues(pointIndex, timeStep);
			}

			return result;
		}

		/// <summary>
		/// Converts every state to a waveform. Waveforms built have <paramref name="pointsCount"/> points and time increment of
		/// <paramref name="timeStep"/>.
		/// </summary>
		/// <returns></returns>
		/// <param name="pointsCount"></param>
		/// <param name="timeStep"></param>
		public WaveformPartialState ToWaveform(int pointsCount, double timeStep)
		{
			// Create state for result
			var result = new WaveformPartialState(_NodeIndices, _ActiveComponentsIndices, States.Keys);

			// For each state
			foreach (var state in States)
			{
				// Use its method to convert to a waveform
				result.States[state.Key] = States[state.Key].ToWaveform(pointsCount, timeStep);
			}

			return result;
		}

		/// <summary>
		/// Builds an <see cref="IPhasorDomainSignal"/> for each node, takes every state from
		/// <see cref="GenericPartialStates{TState, TValues}.States"/>.
		/// </summary>
		/// <param name="includeReferenceNode">If true, an empty signal will be generated for the reference node</param>
		/// <returns></returns>
		public IDictionary<int, IPhasorDomainSignal> PotentialsToPhasorDomainSignal(bool includeReferenceNode)
		{
			// Create a dictionary, keys are node indices, values are mutable time domain signals
			var result = new Dictionary<int, IPhasorDomainSignalMutable>();

			// Add each node index with time domain signal for it
			foreach (var index in _NodeIndices)
			{
				result.Add(index, IoC.Resolve<IPhasorDomainSignalMutable>());
			}

			// Add the states
			foreach (var state in States.Values)
			{
				state.AddPotentialsTo(result);
			}

			// If it was requested to add reference node
			if (includeReferenceNode)
			{
				// Create an entry for it with an empty ITimeDomainSignal stored
				result.Add(AdmittanceMatrixFactory.ReferenceNode, IoC.Resolve<IPhasorDomainSignalMutable>());
			}

			// Cast the mutable time domain signal to standard ITimeDomainSignal
			return result.ToDictionary((x) => x.Key, ((x) => (IPhasorDomainSignal)x.Value));
		}

		/// <summary>
		/// Builds an <see cref="IPhasorDomainSignal"/> for each active component current, takes every state from
		/// <see cref="GenericPartialStates{TState, TValues}.States"/>.
		/// </summary>
		public IDictionary<int, IPhasorDomainSignal> CurrentsToPhasorDomainSignal()
		{
			// Create a dictionary, keys are node indices, values are mutable time domain signals
			var result = new Dictionary<int, IPhasorDomainSignalMutable>();

			// Add each node index with time domain signal for it
			foreach (var index in _ActiveComponentsIndices)
			{
				result.Add(index, IoC.Resolve<IPhasorDomainSignalMutable>());
			}

			// Add the states
			foreach (var state in States.Values)
			{
				state.AddCurrentsTo(result);
			}

			// Cast the mutable time domain signal to standard ITimeDomainSignal
			return result.ToDictionary((x) => x.Key, ((x) => (IPhasorDomainSignal)x.Value));
		}

		#endregion
	}
}