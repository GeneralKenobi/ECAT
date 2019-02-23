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
	}
}