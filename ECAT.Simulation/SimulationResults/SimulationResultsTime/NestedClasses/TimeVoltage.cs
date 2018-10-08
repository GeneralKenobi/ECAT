using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

			/// <summary>
			/// If <see cref="_Cache"/> does not contain an entry with key given by <paramref name="nodeAIndex"/> and
			/// <paramref name="nodeBIndex"/>, caches <paramref name="signal"/>, otherwise doesn't do anything
			/// </summary>
			/// <param name="signal"></param>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			private void CacheHelper(ITimeDomainSignal signal, int nodeAIndex, int nodeBIndex)
			{
				if (!_Cache.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
				{
					_Cache.Add(new Tuple<int, int>(nodeAIndex, nodeBIndex),
						Tuple.Create(
							signal,
							IoC.Resolve<ISignalInformation>(signal, IoC.Resolve<ICommonSignalDescriptions>().Voltage)));
				}
			}

			/// <summary>
			/// Caches the <paramref name="signal"/> as well as its negated copy (with inverted indexes) into <see cref="_Cache"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			private void CacheVoltageDrop(ITimeDomainSignal signal, int nodeAIndex, int nodeBIndex)
			{
				// Cache the original
				CacheHelper(signal, nodeAIndex, nodeBIndex);

				// And cache the reversed one
				CacheHelper(signal.CopyAndNegate(), nodeBIndex, nodeAIndex);
			}

			/// <summary>
			/// Finds all AC voltage waveforms between the two node potentials collections
			/// </summary>
			/// <param name="nodeAACPotentials"></param>
			/// <param name="nodeBACPotentials"></param>
			/// <returns></returns>
			private IEnumerable<KeyValuePair<double, Complex>> GetACWaveforms(IDictionary<double, Complex> nodeAACPotentials,
				IDictionary<double, Complex> nodeBACPotentials)
			{
				// Get the intersecting keys (i.e. find all waveforms that are present at both nodes, in 90% situations it will be all
				// elements but not always)
				var intersectingKeys = nodeAACPotentials.Keys.Intersect(nodeBACPotentials.Keys);

				// For each waveform present at both nodes
				foreach (var key in intersectingKeys)
				{
					// Return a difference between waveform at node B and waveform at node A
					yield return new KeyValuePair<double, Complex>(key, nodeBACPotentials[key] - nodeAACPotentials[key]);
				}

				// For each waveform present only at node B
				foreach (var key in nodeBACPotentials.Keys.Except(intersectingKeys))
				{
					// Add its value to the waveforms
					yield return new KeyValuePair<double, Complex>(key, nodeBACPotentials[key]);
				}

				// For each waveform present only at node A
				foreach (var key in nodeAACPotentials.Keys.Except(intersectingKeys))
				{
					// Subtract its value from the waveforms
					yield return new KeyValuePair<double, Complex>(key, -nodeAACPotentials[key]);
				}
			}

			/// <summary>
			/// Constructs a new <see cref="PhasorDomainSignal"/> based on voltage drop between two nodes (with <paramref name="nodeA"/>
			/// being the reference node). Caches the result (with its negation). Node indexes are assumed to have been checked that
			/// corresponding to them nodes exist in <see cref="_Nodes"/>, if not an exception may be thrown.
			/// </summary>
			/// <param name="nodeA"></param>
			/// <param name="nodeB"></param>
			/// <returns></returns>
			private IPhasorDomainSignal ConstructVoltageDrop(int nodeAIndex, int nodeBIndex)
			{
				return null;
				// Get the nodes
				//var nodeA = _Nodes.First((node) => node.Index == nodeAIndex);
				//var nodeB = _Nodes.First((node) => node.Index == nodeBIndex);

				//// Construct the result
				//var result = IoC.Resolve<IPhasorDomainSignal>(nodeB.DCPotential.Value - nodeA.DCPotential.Value,
				//	GetACWaveforms(nodeA.ACPotentials, nodeB.ACPotentials));

				//// Cache it
				//CacheVoltageDrop(result, nodeA.Index, nodeB.Index);

				//// Return it
				//return result;
			}

			/// <summary>
			/// Checks if voltage drop between <paramref name="nodeAIndex"/> and <paramref name="nodeBIndex"/> can be obtained from
			/// cache (<see cref="_Cache"/>), if not performs all possible actions to create it and cache it. Returns true if, at the
			/// end of the method call, the voltage drop may be obtained from <see cref="_Cache"/>, false otherwise.
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			private bool TryEnableVoltageDrop(int nodeAIndex, int nodeBIndex)
			{
				// Check if nodes exist
				if (NodeExists(nodeAIndex) && NodeExists(nodeBIndex))
				{
					// If it does, check if it was already calculated
					if (!_Cache.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
					{
						ConstructVoltageDrop(nodeAIndex, nodeBIndex);
					}

					// Return success
					return true;
				}
				// If not, assign null and return failure
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Returns true if a node with the given index exists
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			private bool NodeExists(int index) => _NodePotentials.Keys.First((node) => node.Index == index) != null;

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