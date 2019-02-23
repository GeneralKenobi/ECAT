﻿using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for instantenous state of a circuit with respect to one source
	/// </summary>
	public class PhasorState : GenericState<Complex>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public PhasorState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices, ISourceDescription sourceDescription) :
			base(nodeIndices, activeComponentsIndices, sourceDescription) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will be given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public PhasorState(IEnumerable<int> nodeIndices, int activeComponentsCount, ISourceDescription sourceDescription) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), sourceDescription) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodesCount">Number of nodes, indicies available in this instance will be given by a range:
		/// 0 to <paramref name="nodesCount"/></param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will be given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public PhasorState(int nodesCount, int activeComponentsCount, ISourceDescription sourceDescription) :
			this(Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount), sourceDescription) { }

		#endregion
	}
}