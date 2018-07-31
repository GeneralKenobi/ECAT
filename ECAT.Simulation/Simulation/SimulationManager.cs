using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of the simulation module
	/// </summary>
	public class SimulationManager : ISimulationManager
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private methods

		/// <summary>
		/// Finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected terminals), sets their
		/// potential to 0V, removes them from <paramref name="nodes"/>. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		private void ProcessReferenceNodes(List<INode> nodes)
		{
			// Find all reference nodes
			var referenceNodes = FindReferenceNodes(nodes);

			// Set their potential to 0
			referenceNodes.ForEach((node) => node.Potential.Value = 0);

			// Remove them from the nodes list
			nodes.RemoveAll((node) => referenceNodes.Contains(node));
		}

		/// <summary>
		/// Searches through the nodes and finds all reference nodes (nodes that contain an <see cref="IGround"/> in their connected
		/// terminals. If there is no <see cref="IGround"/>, searches through
		/// the nodes and chooses the first node that has a negative terminal of a source connected as the reference node. If there
		/// are no sources then treats all nodes as reference nodes.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns></returns>
		private List<INode> FindReferenceNodes(List<INode> nodes)
		{
			// Filter the nodes, look for all nodes that have na IGround connected to them
			var referenceNodes = new List<INode>(
							nodes.Where((node) => node.ConnectedComponents.Exists((component) => component is IGround)));

			// If any was found, return the list
			if(referenceNodes.Count > 0 )
			{
				return referenceNodes;
			}

			// If not, go through all nodes to find a source
			foreach (var node in nodes)
			{
				// Find all sources connected to the node
				var sources = new List<ITwoTerminal>(node.ConnectedComponents.Where((component) =>
					component is IVoltageSource || component is ICurrentSource).Select((source) => source as ITwoTerminal));

				// Go through each source
				foreach (var source in sources)
				{
					// If it's negative (A) terminal is connected to the node, return the node as the reference node
					if (node.ConnectedTerminals.Contains(source.TerminalA))
					{
						return new List<INode>() { node };
					}
				}
			}
			
			// If no source was found return all nodes as there is no current in the circuit
			return new List<INode>(nodes);
		}
		
		#endregion

		#region Public methods

		/// <summary>
		/// Performs a single DC sweep for the given schematic
		/// </summary>
		/// <param name="schematic"></param>
		public void DCBias(ISchematic schematic)
		{
			// Create a stopwatch to measure the duration of procedures 
			Stopwatch watch = new Stopwatch();

			// Start it for admittance matrix creation
			watch.Start();

			// Generate nodes using helper class
			var nodes = NodeGenerator.Generate(schematic);

			// Assign the potentials from the nodes to the associated terminals
			nodes.ForEach((node) => node.ConnectedTerminals.ForEach((terminal) => terminal.Potential = node.Potential));

			// Find, assign and remove the reference (ground) nodes
			ProcessReferenceNodes(nodes);

			// Construct a DC admittance matrix
			var admittanceMatrix = DCAdmittanceMatrix.Construct(nodes, new List<IVoltageSource>(schematic.Components.Where(
				(component) => component is IVoltageSource).Select((component) => component as IVoltageSource)));

			// Log the success and duration
			IoC.Log($"Constructed DC Admittance Matrix in {watch.ElapsedMilliseconds}ms", InfoLoggerMessageDuration.Short);
						
			watch.Restart();

			try
			{
				// Solve it (for now try-catch for debugging)
				admittanceMatrix.Solve();

				IoC.Log($"Calcualted the result in {watch.ElapsedMilliseconds}ms", InfoLoggerMessageDuration.Short);
			}
			catch (Exception e) { }

			watch.Reset();
		}

		#endregion
	}
}