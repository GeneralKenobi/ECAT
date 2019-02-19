using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for instantenous state of a circuit with respect to one source
	/// </summary>
	public class InstantenousState : GenericState<double>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public InstantenousState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices, ISourceDescription sourceDescription) :
			base(nodeIndices, activeComponentsIndices, sourceDescription) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public InstantenousState(IEnumerable<int> nodeIndices, int activeComponentsCount, ISourceDescription sourceDescription) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), sourceDescription) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Adds the <paramref name="other"/> state to this instance.
		/// </summary>
		/// <param name="other">Object to add to this instance, internal collections' keys have to match, otherwise an exception is thrown</param>
		/// <param name="invalidateSource">If true, <see cref="GenericState{T}.SourceDescription"/> is invalidated (set to null)</param>
		public void AddState(InstantenousState other, bool invalidateSource = true)
		{
			// Check if keys of the other instance match internal keys, if not throw an exception
			if(!(Potentials.Keys.IsSequenceEqual(other.Potentials.Keys) && Currents.Keys.IsSequenceEqual(other.Currents.Keys)))
			{
				throw new ArgumentException(nameof(other) + " has incompatible internal collections (keys don't match)");
			}

			// Add potentials from the other instance
			foreach (var key in Potentials.Keys.ToList())
			{
				Potentials[key] += other.Potentials[key];
			}

			// Add currents from the other instance
			foreach (var key in Currents.Keys.ToList())
			{
				Currents[key] += other.Currents[key];
			}

			if(invalidateSource)
			{
				SourceDescription = null;
			}
		}

		#endregion
	}
}