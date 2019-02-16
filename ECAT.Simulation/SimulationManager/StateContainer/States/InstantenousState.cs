using CSharpEnhanced.Helpers;
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
		public InstantenousState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices) : base(nodeIndices, activeComponentsIndices) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		public InstantenousState(IEnumerable<int> nodeIndices, int activeComponentsCount) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount)) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Adds the <paramref name="other"/> state to this instance.
		/// </summary>
		/// <param name="other">Object to add to this instance, internal collections' keys have to match, otherwise an exception is thrown</param>
		public void AddState(InstantenousState other)
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
		}

		#endregion
	}
}