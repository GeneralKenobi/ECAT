using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	internal class SignalCache<TKey, TValue>
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public SignalCache()
		{
			_Cache = new Dictionary<TKey, TValue>();
		}

		/// <summary>
		/// Default constructor, requires nodes (can't be null) using which voltage drops will be calculated
		/// </summary>
		/// <param name="nodes">Nodes using which voltage drops will be calculated, can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		public SignalCache(IEqualityComparer<TKey> equalityComparer)
		{
			_Cache = new Dictionary<TKey, TValue>(equalityComparer);
		}

		#endregion

		#region Private properties

		/// <summary>
		/// List with all nodes upon which specific results are calculated
		/// </summary>
		private List<INode> _Nodes { get; }

		/// <summary>
		/// Dictionary holding already computed voltage drops. Ints in key tuple are indexes of nodes (Item1 for the first node
		/// (reference node) and Item2 for the second node (target node)). First item in value is <see cref="PhasorDomainSignal"/>
		/// representing the voltage drop, second one is an <see cref="SignalInformation"/> built based on Item1.
		/// </summary>
		private Dictionary<TKey, TValue> _Cache { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// If <see cref="_Cache"/> does not contain an entry with key given by <paramref name="nodeAIndex"/> and
		/// <paramref name="nodeBIndex"/>, caches <paramref name="signal"/>, otherwise doesn't do anything
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		private void CacheHelper(TKey key, TValue value)
		{

		}

		/// <summary>
		/// Caches the <paramref name="signal"/> as well as its negated copy (with inverted indexes) into <see cref="_Cache"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		private void Cache(TKey key, TValue value)
		{
			// Cache the original
			CacheHelper(signal, nodeAIndex, nodeBIndex);

			// And cache the reversed one
			CacheHelper(signal.CopyAndNegate(), nodeBIndex, nodeAIndex);
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
			//
			//// Construct the result
			//var result = IoC.Resolve<IPhasorDomainSignal>(nodeB.DCPotential.Value - nodeA.DCPotential.Value,
			//	GetACWaveforms(nodeA.ACPotentials, nodeB.ACPotentials));
			//
			//// Cache it
			//CacheVoltageDrop(result, nodeA.Index, nodeB.Index);
			//
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
			return true;
			//// Check if nodes exist
			//if (NodeExists(nodeAIndex) && NodeExists(nodeBIndex))
			//{
			//	// If it does, check if it was already calculated
			//	if (!_Cache.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
			//	{
			//		ConstructVoltageDrop(nodeAIndex, nodeBIndex);
			//	}
			//
			//	// Return success
			//	return true;
			//}
			//// If not, assign null and return failure
			//else
			//{
			//	return false;
			//}
		}
		
		#endregion
	}
}