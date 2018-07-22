using ECAT.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of the simulation module
	/// </summary>
	public class SimulationManager : ISimulationManager
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public SimulationManager()
		{
			
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public methods

		public void ConstructDCAdmittanceMatrix(ISchematic schematic)
		{
			var nodes = GenerateNodes(new List<IBaseComponent>(schematic.Components));

			nodes = MergeNodesConnectedByWire(nodes, new List<IWire>(schematic.Wires));
		}

		#endregion

		#region Private methods

		private List<INode> MergeNodesConnectedByWire(List<INode> nodes, List<IWire> wires)
		{
			return null;
		}

		/// <summary>
		/// Generates nodes based on a given schematic, groups all elements that have a <see cref="ITerminal"/> on the same
		/// position in a separate node
		/// </summary>
		/// <param name="schematic"></param>
		/// <returns></returns>
		private List<INode> GenerateNodes(List<IBaseComponent> components)
		{
			// Create a dictionary to store base components assigned to their terminals
			var terminalsWithComponents = new Dictionary<ITerminal, IBaseComponent>();

			// Finally create a list for the generated nodes
			var nodes = new List<INode>();

			// Create a dictionary with Terminals as entries and BaseComponents as values
			components.ForEach((component) => 
				component.GetTerminals().ForEach((terminal) =>
				terminalsWithComponents.Add(terminal, component)));

			// Go through all entries, create new Nodes by grouping all components that have a terminal on the same position
			// in one node
			while(terminalsWithComponents.Count > 0)
			{
				// Get the position of the terminal at index 0
				var currentPosition = terminalsWithComponents.ElementAt(0).Key.Position;

				// Create a new node
				var node = new Node();

				node.ConnectedComponents = 
					// Filter the dictionary to find all entries with terminals on the previously determined position,
					new List<IBaseComponent>(terminalsWithComponents.Where((entry) => entry.Key.Position == currentPosition).
					// then project them to only the values (IBaseComponents)
					Select((x) => x.Value));

				// Assign the reference to value of potential at the respective node
				node.ConnectedComponents.ForEach((component) => component.GetTerminals().ForEach((terminal) =>
					terminal.Potential = node.Potential));

				nodes.Add(node);
			}

			return nodes;
		}

		#endregion
	}
}