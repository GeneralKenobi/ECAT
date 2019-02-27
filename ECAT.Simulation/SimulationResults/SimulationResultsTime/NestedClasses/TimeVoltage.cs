using CSharpEnhanced.Helpers;
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
		private class TimeVoltage : VoltageCache<ITimeDomainSignal>, IVoltageDB, IVoltageSignalDB<ITimeDomainSignal>
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
			public TimeVoltage(IEnumerable<KeyValuePair<int, ITimeDomainSignal>> data) : base(data) { }

			#endregion

			#region Protected methods

			/// <summary>
			/// Constructs a new <see cref="PhasorDomainSignal"/> based on voltage drop between two nodes (with <paramref name="nodeA"/>
			/// being the reference node). Caches the result (with its negation). Node indexes are assumed to have been checked that
			/// corresponding to them nodes exist in <see cref="_Nodes"/>, if not an exception may be thrown.
			/// </summary>
			/// <param name="nodeA"></param>
			/// <param name="nodeB"></param>
			/// <returns></returns>
			protected override ITimeDomainSignal ConstructVoltageDrop(int nodeAIndex, int nodeBIndex)
			{
				// Get the nodes
				var nodeA = _Data[nodeAIndex];
				var nodeB = _Data[nodeBIndex];

				//var nodeAWaveforms = nodeA.AllWaveforms.ToDictionary((x) => x.Key, (x) => x.Value);
				//var nodeBWaveforms = nodeB.AllWaveforms.ToDictionary((x) => x.Key, (x) => x.Value);

				// Construct a result
				var result = IoC.Resolve<ITimeDomainSignalMutable>(nodeA.Samples, nodeA.TimeStep, nodeA.StartTime);

				// Add waveforms from node B
				nodeB.AllWaveforms.ForEach((x) => result.AddWaveform(x.Key, x.Value));

				// And subtract waveforms from node A
				nodeA.AllWaveforms.ForEach((x) => result.AddWaveform(x.Key, x.Value.Select((y) => -y)));

				//// Add waveforms of sources only appearing on node B (unlikely any will appear but it can't be neglected)
				//foreach (var key in nodeBWaveforms.Keys.Except(nodeAWaveforms.Keys))
				//{
				//	result.AddWaveform(key, nodeBWaveforms[key]);
				//}

				//// Subtract waveforms of sources only appearing on node B (unlikely any will appear but it can't be neglected)
				//foreach (var key in nodeAWaveforms.Keys.Except(nodeBWaveforms.Keys))
				//{
				//	result.AddWaveform(key, nodeAWaveforms[key].Select((x) => -x));
				//}

				//// Add waveforms for souces appearing on both nodes - add values from node B and subtract values from node A
				//nodeAWaveforms.Keys.Intersect(nodeBWaveforms.Keys).
				//	ForEach((source) => result.AddWaveform(source, nodeBWaveforms[source].MergeSelect(nodeAWaveforms[source], (x, y) => x - y)));

				return result;
			}

			/// <summary>
			/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <returns></returns>
			protected override ITimeDomainSignal CopyAndNegate(ITimeDomainSignal signal) => signal.CopyAndNegate();

			#endregion

			#region Public methods

			#region IVoltageSignalDB

			/// <summary>
			/// Gets voltage drop of a node with respect to ground or null if unsuccessful and assigns it to <paramref name="voltage"/>.
			/// Returns true on success, false otherwise.
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="voltage"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public bool TryGet(int nodeIndex, out ITimeDomainSignal voltage, bool nodeToGround = true) => nodeToGround ?
				TryGet(SimulationManager.GroundNodeIndex, nodeIndex, out voltage) :
				TryGet(nodeIndex, SimulationManager.GroundNodeIndex, out voltage);

			/// <summary>
			/// Gets voltage drop between two nodes (with node A being treated as the reference node) or null if unsuccessful and
			/// assigns it to <paramref name="voltage"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <param name="voltage"></param>
			/// <returns></returns>
			public bool TryGet(int nodeAIndex, int nodeBIndex, out ITimeDomainSignal voltage)
			{
				// Check if it's possible to get the voltage drop from cache
				if (TryEnableVoltageDrop(nodeAIndex, nodeBIndex) &&
					// If the first condition returned true, the element should be in cache but check so as not to crash by accident
					_Cache.TryGetValue(new Tuple<int, int>(nodeAIndex, nodeBIndex), out var voltagePackage))
				{
					// Return the result
					voltage = voltagePackage.Item1;
					return true;
				}
				// If not return null
				else
				{
					voltage = null;
					return false;
				}
			}

			/// <summary>
			/// Gets voltage drop across a <see cref="ITwoTerminal"/> component or null if unsuccessful and assigns it to
			/// <paramref name="voltage"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <param name="voltage"></param>
			/// <returns></returns>
			public bool TryGet(ITwoTerminal component, out ITimeDomainSignal voltage, bool voltageBA = true) => voltageBA ?
				TryGet(component.TerminalA.NodeIndex, component.TerminalB.NodeIndex, out voltage) :
				TryGet(component.TerminalB.NodeIndex, component.TerminalA.NodeIndex, out voltage);

			#endregion

			#region IVoltageDB

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground or returns null if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeIndex, bool nodeToGround = true) => nodeToGround ?
				Get(SimulationManager.GroundNodeIndex, nodeIndex) : Get(nodeIndex, SimulationManager.GroundNodeIndex);
			
			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns null
			/// if unsuccessful
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeAIndex, int nodeBIndex)
			{
				// Check if it's possible to get the voltage drop from cache
				if (TryEnableVoltageDrop(nodeAIndex, nodeBIndex) &&
					// If the first condition returned true, the element should be in cache but check so as not to crash by accident
					_Cache.TryGetValue(new Tuple<int, int>(nodeAIndex, nodeBIndex), out var voltagePackage))
				{
					// Return the result
					return voltagePackage.Item2;
				}
				// If not return null
				else
				{
					return null;
				}
			}

			/// <summary>
			/// Gets information on voltage drop across a <see cref="ITwoTerminal"/> component
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <returns></returns>
			public ISignalInformation Get(ITwoTerminal component, bool voltageBA = true) => voltageBA ?
				Get(component.TerminalA.NodeIndex, component.TerminalB.NodeIndex) :
				Get(component.TerminalB.NodeIndex, component.TerminalA.NodeIndex);

			#endregion

			#endregion
		}
	}
}