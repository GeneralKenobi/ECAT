using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for state of a circuit represented by waveforms
	/// </summary>
	public class WaveformState : GenericState<IList<double>>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public WaveformState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices, ISourceDescription sourceDescription) :
			base(nodeIndices, activeComponentsIndices, sourceDescription, () => new List<double>()) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will be given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public WaveformState(IEnumerable<int> nodeIndices, int activeComponentsCount, ISourceDescription sourceDescription) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), sourceDescription) { }

		#endregion

		#region Private methods

		/// <summary>
		/// Adds this instance (passed dictionary) to <paramref name="signal"/>
		/// </summary>
		/// <param name="signal">Signal to add to</param>
		/// <param name="sourceCollection">Source dictionary to add from (<see cref="GenericState{T}.Potentials"/> or <see cref="GenericState{T}.Currents"/>
		/// </param>
		private void AddToSignalHelper(IDictionary<int, ITimeDomainSignalMutable> signal, IDictionary<int, IList<double>> sourceCollection)
		{
			// Perform necessary null checks
			if(signal == null || sourceCollection == null)
			{
				throw new ArgumentNullException();
			}

			if(SourceDescription == null)
			{
				throw new Exception(nameof(SourceDescription) + " cannot be null for this method (it's needed to determine the source of the waveform" +
					" in order to add to appropriate collection - AC/DC)");
			}

			// Check if key collections match - it's necessary, otherwise the signal is not compatible with this instance
			if (!sourceCollection.Keys.IsSequenceEqual(signal.Keys))
			{
				throw new ArgumentException(nameof(signal) + " has incompatible key collection");
			}

			// For each key in the dictionary
			foreach(var key in sourceCollection.Keys)
			{
				// Add that entry's value to appropriate signal
				signal[key].AddWaveform(SourceDescription, sourceCollection[key]);
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Adds this instance's <see cref="GenericState{T}.Potentials"/> to <paramref name="signal"/>
		/// </summary>
		/// <param name="signal">Signal to add to</param>
		public void AddPotentialsTo(IDictionary<int, ITimeDomainSignalMutable> signal) => AddToSignalHelper(signal, Potentials);

		/// <summary>
		/// Adds this instance's <see cref="GenericState{T}.Currents"/> to <paramref name="signal"/>
		/// </summary>
		/// <param name="signal">Signal to add to</param>
		public void AddCurrentsTo(IDictionary<int, ITimeDomainSignalMutable> signal) => AddToSignalHelper(signal, Currents);

		#endregion
	}
}