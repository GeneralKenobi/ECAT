using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Base class that may be used when implementing <see cref="IVoltageDB"/> that allows for easy caching of voltage drops.
	/// </summary>
	/// <typeparam name="TSignal">Type of signal that is created</typeparam>
	internal abstract class VoltageCache<TSignal> : SignalCache<Tuple<int, int>, TSignal> where TSignal : ISignalData
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="data">Data obtained during simulation; potentials at nodes</param>
		/// <exception cref="ArgumentNullException"></exception>
		protected VoltageCache(IEnumerable<KeyValuePair<INode, TSignal>> data)
			// Compare the elements of the Tuples, not tuples themselves
			: base (new CustomEqualityComparer<Tuple<int, int>>((x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2))
		{
			if(data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			_Data = new Dictionary<INode, TSignal>(data.ToDictionary((x) => x.Key, (x) => x.Value));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Data obtained during simulation
		/// </summary>
		protected Dictionary<INode, TSignal> _Data { get; }

		#endregion

		#region Protected methods

		/// <summary>
		/// If <see cref="_Cache"/> does not contain an entry with key given by <paramref name="nodeAIndex"/> and
		/// <paramref name="nodeBIndex"/>, caches <paramref name="signal"/>, otherwise doesn't do anything
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		protected void CacheHelper(TSignal signal, int nodeAIndex, int nodeBIndex)
		{
			if (!_Cache.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
			{
				_Cache.Add(new Tuple<int, int>(nodeAIndex, nodeBIndex),
					Tuple.Create(signal, IoC.Resolve<ISignalInformation>(signal, IoC.Resolve<ICommonSignalDescriptions>().Voltage)));
			}
		}

		/// <summary>
		/// Caches the <paramref name="signal"/> as well as its negated copy (with inverted indexes) into <see cref="_Cache"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		protected void CacheVoltageDrop(TSignal signal, int nodeAIndex, int nodeBIndex)
		{
			// Cache the original
			CacheHelper(signal, nodeAIndex, nodeBIndex);

			// And cache the reversed one
			CacheHelper(CopyAndNegate(signal), nodeBIndex, nodeAIndex);
		}

		/// <summary>
		/// Constructs and caches voltage drop between the two nodes (assumes both nodes exist)
		/// </summary>
		/// <param name="nodeAIndex">Reference node</param>
		/// <param name="nodeBIndex"></param>
		protected void ConstructAndCacheVoltageDrop(int nodeAIndex, int nodeBIndex) =>
			CacheVoltageDrop(ConstructVoltageDrop(nodeAIndex, nodeBIndex), nodeAIndex, nodeBIndex);

		/// <summary>
		/// Checks if voltage drop between <paramref name="nodeAIndex"/> and <paramref name="nodeBIndex"/> can be obtained from
		/// cache (<see cref="_Cache"/>), if not performs all possible actions to create it and cache it. Returns true if, at the
		/// end of the method call, the voltage drop may be obtained from <see cref="_Cache"/>, false otherwise.
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <returns></returns>
		protected bool TryEnableVoltageDrop(int nodeAIndex, int nodeBIndex)
		{
			// Check if nodes exist
			if (NodeExists(nodeAIndex) && NodeExists(nodeBIndex))
			{
				// If it does, check if it was already calculated
				if (!_Cache.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
				{
					CacheVoltageDrop(ConstructVoltageDrop(nodeAIndex, nodeBIndex), nodeAIndex, nodeBIndex);
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
		protected bool NodeExists(int index) => _Data.Keys.FirstOrDefault((node) => node.Index == index) != null;

		/// <summary>
		/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		protected abstract TSignal CopyAndNegate(TSignal signal);

		/// <summary>
		/// Constructs a new <see cref="PhasorDomainSignal"/> based on voltage drop between two nodes (with <paramref name="nodeA"/>
		/// being the reference node). Node indexes are assumed to have been checked that
		/// corresponding to them nodes exist in <see cref="_Nodes"/>, if not an exception may be thrown.
		/// </summary>
		/// <param name="nodeA"></param>
		/// <param name="nodeB"></param>
		/// <returns></returns>
		protected abstract TSignal ConstructVoltageDrop(int nodeAIndex, int nodeBIndex);

		#endregion
	}
}