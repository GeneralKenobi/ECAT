using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for generic system state description (potentials and active components currents).
	/// </summary>
	/// <typeparam name="T">Type used to represent value of potentials and currents</typeparam>
	public class GenericState<T>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="defaultValueFactory">Func used for generating initial values in <see cref="Potentials"/> and <see cref="Currents"/>,
		/// if null (which is the default value) <see cref="default(T)"/> will be used</param>
		public GenericState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices, Func<T> defaultValueFactory = null)
		{
			// Make an entry for each node
			foreach (var node in nodeIndices)
			{
				Potentials.Add(node, defaultValueFactory == null ? default(T) : defaultValueFactory());
			}

			// Make an entry for each index
			foreach (var index in activeComponentsIndices)
			{
				Currents.Add(index, defaultValueFactory == null ? default(T) : defaultValueFactory());
			}
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="defaultValueFactory">Func used for generating initial values in <see cref="Potentials"/> and <see cref="Currents"/>,
		/// if null (which is the default value) <see cref="default(T)"/> will be used</param>
		public GenericState(IEnumerable<int> nodeIndices, int activeComponentsCount, Func<T> defaultValueFactory = null) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), defaultValueFactory) { }

		#endregion

		#region Public properties

		/// <summary>
		/// Contains nodes and their potentials
		/// </summary>
		public IDictionary<int, T> Potentials { get; } = new Dictionary<int, T>();

		/// <summary>
		/// Contains indices of active components and their currents
		/// </summary>
		public IDictionary<int, T> Currents { get; } = new Dictionary<int, T>();

		#endregion
	}
}