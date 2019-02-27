using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias : ISimulationResults
	{
		/// <summary>
		/// Provides functionality connected with storing, calculating and exposing voltage drops and information about them in
		/// form of <see cref="IPhasorDomainSignal"/>s and <see cref="ISignalInformation"/>
		/// </summary>
		private class BiasVoltage : VoltageCache<IPhasorDomainSignal>, IVoltageDB, IVoltageSignalDB<IPhasorDomainSignal>
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires nodes (can't be null) using which voltage drops will be calculated
			/// </summary>
			/// <param name="nodes">Nodes using which voltage drops will be calculated, can't be null</param>
			/// <exception cref="ArgumentNullException"></exception>
			public BiasVoltage(IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> nodes) : base(nodes) { }

			#endregion

			#region Private methods

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
			protected override IPhasorDomainSignal ConstructVoltageDrop(int nodeAIndex, int nodeBIndex)
			{
				// Get the nodes
				var nodeA = _Data[nodeAIndex];
				var nodeB = _Data[nodeBIndex];

				// Construct the result
				var result = IoC.Resolve<IPhasorDomainSignal>(nodeB.DC - nodeA.DC, GetACWaveforms(nodeA.Phasors, nodeB.Phasors));

				// Return it
				return result;
			}

			/// <summary>
			/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <returns></returns>
			protected override IPhasorDomainSignal CopyAndNegate(IPhasorDomainSignal signal) => signal.CopyAndNegate();

			#endregion

			#region Public methods

			#region IBiasVoltage interface

			/// <summary>
			/// Gets voltage drop of a node with respect to ground or null if unsuccessful and assigns it to <paramref name="voltage"/>.
			/// Returns true on success, false otherwise.
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="voltage"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public bool TryGet(int nodeIndex, out IPhasorDomainSignal voltage, bool nodeToGround = true) =>
				// Depending on requested voltage drop direction
				nodeToGround ?
				// Get voltage drop from ground to node
				TryGet(AdmittanceMatrixFactory.ReferenceNode, nodeIndex, out voltage) :
				// Get voltage drop from node to ground
				TryGet(nodeIndex, AdmittanceMatrixFactory.ReferenceNode, out voltage);

			/// <summary>
			/// Gets voltage drop between two nodes (with node A being treated as the reference node) or null if unsuccessful and
			/// assigns it to <paramref name="voltage"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <param name="voltage"></param>
			/// <returns></returns>
			public bool TryGet(int nodeAIndex, int nodeBIndex, out IPhasorDomainSignal voltage)
			{
				// Check if it's possible to get the voltage drop from cache
				if (TryEnableVoltageDrop(nodeAIndex, nodeBIndex) &&
					// If the first condition returned true, the element should be in cache but check so as not to crash by accident
					_Cache.TryGetValue(new Tuple<int, int>(nodeAIndex, nodeBIndex), out var voltagePackage))
				{					
					// Assign result and return success
					voltage = voltagePackage.Item1;
					return true;
				}
				// If not, assign null and return failure
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
			public bool TryGet(ITwoTerminal component, out IPhasorDomainSignal voltage, bool voltageBA = true) =>
				// Depending on requested voltage drop direction
				voltageBA ?
				// Get voltage drop from node A to node B
				TryGet(component.TerminalA.NodeIndex, component.TerminalB.NodeIndex, out voltage) :
				// Get voltage drop from node B to node A
				TryGet(component.TerminalB.NodeIndex, component.TerminalA.NodeIndex, out voltage);

			#endregion

			#region IVolateDB interface

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground or returns null if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeIndex, bool nodeToGround = true) =>
				// Depending on requested voltage drop direction
				nodeToGround ?
				// Get voltage drop from ground to node
				Get(AdmittanceMatrixFactory.ReferenceNode, nodeIndex) :
				// Get voltage drop from node to ground
				Get(nodeIndex, AdmittanceMatrixFactory.ReferenceNode);
			
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
			public ISignalInformation Get(ITwoTerminal component, bool voltageBA = true) =>
				// Depending on requested voltage drop direction
				voltageBA ?
				// Get voltage drop from node A to node B
				Get(component.TerminalA.NodeIndex, component.TerminalB.NodeIndex) :
				// Get voltage drop from node B to node A
				Get(component.TerminalB.NodeIndex, component.TerminalA.NodeIndex);

			#endregion

			#endregion
		}
	}
}