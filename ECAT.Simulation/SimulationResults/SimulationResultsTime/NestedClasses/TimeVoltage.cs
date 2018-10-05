using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	partial class SimulationResultsTime
	{
		/// <summary>
		/// Manages voltage-related results
		/// </summary>
		private class TimeVoltage : IVoltageDB
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="nodePotentials">Sequence of pairs, key is a node, values are potentials calculated
			/// for time instants for that node</param>
			/// <param name="startTime">Start time of the simulation</param>
			/// <param name="timeStep">Time step of the simulation - difference between two subsequent simulation points</param>
			/// <exception cref="ArgumentNullException"></exception>
			public TimeVoltage(IEnumerable<KeyValuePair<INode, IEnumerable<double>>> nodePotentials, double timeStep,
				double startTime)
			{
				_NodePotentials = nodePotentials?.ToDictionary((pair) => pair.Key, (pair) => pair.Value) ??
					throw new ArgumentNullException(nameof(nodePotentials));
				_TimeStep = timeStep;
				_StartTime = startTime;
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Instantenous values of potentials at specific nodes
			/// </summary>
			private Dictionary<INode, IEnumerable<double>> _NodePotentials { get; }

			/// <summary>
			/// Time step of the simulation - difference between two subsequent simulation points
			/// </summary>
			private double _TimeStep { get; }

			/// <summary>
			/// Start time of the simulation
			/// </summary>
			private double _StartTime { get; }

			/// <summary>
			/// Dictionary holding already computed voltage drops. Ints in key tuple are indexes of nodes (Item1 for the first node
			/// (reference node) and Item2 for the second node (target node)). First item in value is <see cref="PhasorDomainSignal"/>
			/// representing the voltage drop, second one is an <see cref="SignalInformation"/> built based on Item1.
			/// </summary>
			private Dictionary<Tuple<int, int>, Tuple<ITimeDomainSignal, ISignalInformation>> _Cache { get; } =
				new Dictionary<Tuple<int, int>, Tuple<ITimeDomainSignal, ISignalInformation>>(
					new CustomEqualityComparer<Tuple<int, int>>(
					// Compare the elements of the Tuples, not tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			#endregion

			#region Public methods

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground or returns null if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeIndex, bool nodeToGround = true)
			{
				return null;
			}

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns null
			/// if unsuccessful
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeAIndex, int nodeBIndex)
			{
				return null;
			}

			/// <summary>
			/// Gets information on voltage drop across a <see cref="ITwoTerminal"/> component
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <returns></returns>
			public ISignalInformation Get(ITwoTerminal component, bool voltageBA = true)
			{
				return null;
			}

			#endregion
		}
	}
}