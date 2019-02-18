using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for waveform based state of a system
	/// </summary>
	public class WaveformPartialState : GenericPartialStates<WaveformState, IEnumerable<double>>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public WaveformPartialState(int acSourcesCount, IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			base(acSourcesCount, nodeIndices, activeComponentsIndices, acVoltageSourcesDescriptions, dcVoltageSourcesDescriptions,
				(x, y, z) => new WaveformState(x, y, z))
		{
			
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodeIndices">Indices of nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public WaveformPartialState(int acSourcesCount, IEnumerable<int> nodeIndices, int activeComponentsCount,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			this(acSourcesCount, nodeIndices, Enumerable.Range(0, activeComponentsCount), acVoltageSourcesDescriptions,
				dcVoltageSourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="pointsCount">Number of points in each waveform, nonnegative</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public WaveformPartialState(int acSourcesCount, int nodesCount, int activeComponentsCount,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			this(acSourcesCount, Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount), acVoltageSourcesDescriptions,
				dcVoltageSourcesDescriptions) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="acSourcesCount">Number of AC sources expected in this state</param>
		/// <param name="nodesCount">Number of nodes, indices available in this instance are given by a
		/// range: 0 to <paramref name="nodesCount"/> - 1</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="acVoltageSourcesDescriptions">Descriptions of AC voltage sources that will produce the partial states</param>
		/// <param name="pointsCount">Number of points in each waveform, nonnegative</param>
		/// <param name="dcVoltageSourcesDescriptions">Descriptions of DC voltage sources that will produce the partial states</param>
		public WaveformPartialState(int acSourcesCount, int nodesCount, IEnumerable<int> activeComponentsIndices,
			IEnumerable<IActiveComponentDescription> acVoltageSourcesDescriptions,
			IEnumerable<IActiveComponentDescription> dcVoltageSourcesDescriptions) :
			this(acSourcesCount, Enumerable.Range(0, nodesCount), activeComponentsIndices, acVoltageSourcesDescriptions, dcVoltageSourcesDescriptions) { }

		#endregion

		#region Private methods

		/// <summary>
		/// Returns true if all potentials in <paramref name="waveforms"/> have points count equal to <paramref name="targetPointsCount"/>,
		/// false otherwise
		/// </summary>
		/// <param name="waveforms"></param>
		/// <param name="targetPointsCount"></param>
		/// <returns></returns>
		private bool CheckWaveformsCorrectness(IEnumerable<IEnumerable<double>> waveforms, int targetPointsCount)
		{
			// Check potentials
			foreach (var waveform in waveforms)
			{
				if (waveform.Count() != targetPointsCount)
				{
					return false;
				}
			}
			
			return true;
		}

		/// <summary>
		/// Checks the integrity of all potentials waveforms in this instance - container is correct if there is at least one
		/// <see cref="IACVoltageSource"/> and one node index and all potentials waveforms have the same points count.
		/// If it's the case then the points count is assigned to <paramref name="pointsCount"/> and method returns true, otherwise
		/// <paramref name="pointsCount"/> is assigned -1 and method returns false.
		/// </summary>
		/// <param name="pointsCount"></param>
		/// <returns></returns>
		private bool CheckPotentialsCorrectness(out int pointsCount)
		{
			// Assign -1 initially
			pointsCount = -1;

			// Make sure there's at least one AC voltage source and at least one node index (both are required for correctness)
			if(!(ACStates.Length > 0 && ACStates[0].Potentials.Count > 0))
			{
				return false;
			}

			// Get points count of the first wave - all other waveforms have to have the same points count
			var targetPointsCount = ACStates[0].Potentials.First().Value.Count();

			// Check every AC state
			foreach(var acState in ACStates)
			{
				if(!CheckWaveformsCorrectness(acState.Potentials.Values, targetPointsCount))
				{
					return false;
				}
			}

			// Check DC state
			if (!CheckWaveformsCorrectness(DCState.Potentials.Values, targetPointsCount))
			{
				return false;
			}

			// All waveforms had equal points count - assign it to out variable
			pointsCount = targetPointsCount;
			// And return success
			return true;
		}

		/// <summary>
		/// Checks the integrity of all current waveforms in this instance - container is correct if there is at least one
		/// <see cref="IACVoltageSource"/> and one active component index and all waveforms have the same points count.
		/// If it's the case then the points count is assigned to <paramref name="pointsCount"/> and method returns true, otherwise
		/// <paramref name="pointsCount"/> is assigned -1 and method returns false.
		/// </summary>
		/// <param name="pointsCount"></param>
		/// <returns></returns>
		private bool CheckCurrentsCorrectness(out int pointsCount)
		{
			// Assign -1 initially
			pointsCount = -1;

			// Make sure there's at least one AC voltage source and at least one active component index (both are required for correctness)
			if (!(ACStates.Length > 0 && ACStates[0].Currents.Count > 0))
			{
				return false;
			}

			// Get points count of the first wave - all other waveforms have to have the same points count
			var targetPointsCount = ACStates[0].Currents.First().Value.Count();

			// Check every AC state
			foreach (var acState in ACStates)
			{
				if (!CheckWaveformsCorrectness(acState.Currents.Values, targetPointsCount))
				{
					return false;
				}
			}

			// Check DC state
			if (!CheckWaveformsCorrectness(DCState.Currents.Values, targetPointsCount))
			{
				return false;
			}

			// All waveforms had equal points count - assign it to out variable
			pointsCount = targetPointsCount;
			// And return success
			return true;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Constructs <see cref="ITimeDomainSignal"/>s for every node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<int, ITimeDomainSignal>> PotentialsToTimeDomainSignals(double timeStep)
		{
			// Check if all potentials are matching waveforms
			if(!CheckPotentialsCorrectness(out var pointsCount))
			{
				throw new Exception("Waveforms have differing points counts");
			}

			// Create a dictionary, keys are node indices, values are mutable time domain signals
			var result = new Dictionary<int, ITimeDomainSignalMutable>();

			// Add each node index with time domain signal for it
			foreach (var index in _NodeIndices)
			{
				result.Add(index, IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep));
			}

			// Add the AC states
			foreach(var state in ACStates)
			{
				state.AddPotentialsTo(result);
			}

			// Add the DC state
			DCState.AddPotentialsTo(result);

			// Cast the mutable time domain signal to standard ITimeDomainSignal
			return result.ToDictionary((x) => x.Key, ((x) => (ITimeDomainSignal)x.Value));
		}

		/// <summary>
		/// Constructs <see cref="ITimeDomainSignal"/>s for every node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<int, ITimeDomainSignal>> CurrentsToTimeDomainSignals(double timeStep)
		{
			// Check if all currents are matching waveforms
			if (!CheckCurrentsCorrectness(out var pointsCount))
			{
				throw new Exception("Waveforms have differing points counts");
			}

			// Create a dictionary, keys are active components indices, values are mutable time domain signals
			var result = new Dictionary<int, ITimeDomainSignalMutable>();

			// Add each active component index with time domain signal for it
			foreach (var index in _ActiveComponentsIndices)
			{
				result.Add(index, IoC.Resolve<ITimeDomainSignalMutable>(pointsCount, timeStep));
			}

			// Add the AC states
			foreach (var state in ACStates)
			{
				state.AddCurrentsTo(result);
			}

			// Add the DC state
			DCState.AddCurrentsTo(result);

			// Cast the mutable time domain signal to standard ITimeDomainSignal
			return result.ToDictionary((x) => x.Key, ((x) => (ITimeDomainSignal)x.Value));
		}

		#endregion
	}
}