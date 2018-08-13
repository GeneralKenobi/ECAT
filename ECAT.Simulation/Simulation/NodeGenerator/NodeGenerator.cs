using ECAT.Core;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Helper class for <see cref="SimulationManager"/>, takes a 
	/// </summary>
	public static class NodeGenerator
    {
		#region Private static methods		

		/// <summary>
		/// Merges all nodes that are connected by a wire network (modifies both lists)
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="wires"></param>
		/// <returns></returns>
		private static List<INode> MergeNodesConnectedByWires(List<INode> nodes, List<IWire> wires)
		{
			// Keep going until all wires were considered
			while (wires.Count > 0)
			{
				// Get a list of all wires that are connected to the first wire
				// to it
				var wireNetwork = new List<IWire>(wires[0].GetAllConnectedWires(wires));

				// Get all nodes that are associated with the ends of those wires (at this point nodes may have null positions which
				// means those nodes are no longer position-based, so check for that)
				var associatedNodes = new List<INode>(nodes.Where((node) => node.Position != null && wireNetwork.Exists((wire) =>
					wire.BelongsToWire(node.Position))));

				// Create a node which will cover the whole obtained wire network
				var wireNode = new Node();

				// Merge all found nodes
				associatedNodes.ForEach((node) => wireNode.Merge(node));

				// Remove them from the collection
				associatedNodes.ForEach((node) => nodes.Remove(node));

				// Add the node that remained after merging to the collection
				nodes.Add(wireNode);

				// Remove all wires that were handled in the process
				wireNetwork.ForEach((wire) => wires.Remove(wire));
			}

			return nodes;
		}

		/// <summary>
		/// Generates nodes based on a given schematic, groups all elements that have a <see cref="ITerminal"/> on the same
		/// position in a single node
		/// </summary>
		/// <param name="schematic"></param>
		/// <returns></returns>
		private static List<INode> GenerateSamePositionNodes(List<IBaseComponent> components)
		{
			// Create a dictionary to store base components assigned to their terminals
			var terminalsWithComponents = new Dictionary<ITerminal, IBaseComponent>();

			// Create a list for the generated nodes
			var nodes = new List<INode>();

			// Create a dictionary with Terminals as entries and BaseComponents as values			
			components.ForEach((component) =>
			{
				foreach (var terminal in component.GetTerminals())
				{
					terminalsWithComponents.Add(terminal, component);
				}
			});

			// Go through all entries, create new Nodes by grouping all components that have a terminal on the same position
			// in one node
			while (terminalsWithComponents.Count > 0)
			{
				nodes.Add(GenerateNode(terminalsWithComponents));
			}

			return nodes;
		}

		/// <summary>
		/// Helper of <see cref="GenerateSamePositionNodes(List{IBaseComponent})"/>. Finds all <see cref="ITerminal"/>s in the dictionary
		/// that have the same <see cref="IPlanePosition"/> as the element at index 0, creates a new <see cref="INode"/> based on them,
		/// removes those <see cref="ITerminal"/>s from the dictionary and returns the <see cref="INode"/>
		/// </summary>
		/// <param name="terminalsWithComponents"></param>
		/// <returns></returns>
		private static INode GenerateNode(Dictionary<ITerminal, IBaseComponent> terminalsWithComponents)
		{
			// Get the position of the terminal at index 0
			var currentPosition = terminalsWithComponents.ElementAt(0).Key.Position;

			// Create a new node with the assumed position
			var node = new Node(currentPosition);

			// Filter the dictionary to find all entries with terminals on the previously determined position,
			var validEntries = new List<KeyValuePair<ITerminal, IBaseComponent>>(
				terminalsWithComponents.Where((entry) => entry.Key.Position.Equals(currentPosition)));

			// Project them to values (IBaseComponents) for the component list
			node.ConnectedComponents = new List<IBaseComponent>(validEntries.Select((entry) => entry.Value));

			// And to keys for the terminal list (ITerminal)
			node.ConnectedTerminals = new List<ITerminal>(validEntries.Select((entry) => entry.Key));

			// Remove all handled ITerminals from the dictionary
			validEntries.ForEach((item) => terminalsWithComponents.Remove(item.Key));

			return node;
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Generates nodes for all components, one node connects all terminals (components) that have the same position or that
		/// are connected through wire(s)
		/// </summary>
		/// <param name="schematic"></param>
		/// <returns></returns>
		public static List<INode> Generate(ISchematic schematic)
		{
			// Generate nodes located on the same position
			var nodes = GenerateSamePositionNodes(new List<IBaseComponent>(schematic.Components));

			// Merge them based on the wire connections
			nodes = MergeNodesConnectedByWires(nodes, new List<IWire>(schematic.Wires));

			return nodes;
		}

		#endregion
	}
}