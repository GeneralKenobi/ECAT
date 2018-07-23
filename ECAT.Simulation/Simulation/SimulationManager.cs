using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
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

		public void SingleDCSweep(ISchematic schematic)
		{
			var nodes = GenerateNodes(new List<IBaseComponent>(schematic.Components));

			nodes = MergeNodesConnectedByWire(nodes, new List<IWire>(schematic.Wires));

			// Assign the reference to value of potential at the respective node
			//nodes.ForEach((node) => node.ConnectedComponents.ForEach((component) => component.Item1.GetTerminals().ForEach((terminal) =>
			//	terminal.Potential = node.Potential)));

			nodes.ForEach((node) => node.ConnectedTerminals.ForEach((terminal) => terminal.Potential = node.Potential));

			var referenceNode = nodes.Find((x) => x.ConnectedComponents.Exists((y) => y is ICurrentSource cs &&
				x.ConnectedTerminals.Contains(cs.TerminalA)));

			nodes.Remove(referenceNode);
			
			int nonGroundNodes = nodes.Count;

			var coefficients = new IExpression[nonGroundNodes, nonGroundNodes];
			var currents = new IExpression[nonGroundNodes];
			
			for(int i=0; i<nonGroundNodes; ++i)
			{
				for (int j = 0; j < nonGroundNodes; ++j)
				{
					coefficients[i, j] = Variable.Zero;
				}

				currents[i] = Variable.Zero;
			}


			for(int i = 0; i<nonGroundNodes; ++i)
			{
				for (int j = 0; j < nonGroundNodes; ++j)
				{
					if (i == j)
					{
						nodes[i].ConnectedComponents.ForEach((x) =>
						{
							if (x is ITwoTerminal twoTerminal)
							{
								coefficients[i, j] = coefficients[i, j].Add(new Variable.VariableSource(twoTerminal.Admittance.Real).Variable);
							}
							else throw new NotImplementedException();
						});
					}
					else
					{
						foreach(var item in nodes[i].ConnectedComponents.Intersect(nodes[j].ConnectedComponents))
						{
							if (item is ITwoTerminal twoTerminal)
							{
								coefficients[i, j] = coefficients[i, j].Subtract(new Variable.VariableSource(twoTerminal.Admittance.Real).Variable);
							}
							else throw new NotImplementedException();
						}
					}
				}
			}

			for(int i=0; i<nonGroundNodes; ++i)
			{
				foreach(var item in nodes[i].ConnectedComponents)
				{
					if (item is ICurrentSource source)
					{
						if (nodes[i].ConnectedTerminals.Contains(source.TerminalA))
						{
							currents[i] = currents[i].Subtract(new Variable.VariableSource(source.ProducedCurrent).Variable);
						}
						else
						{
							currents[i] = currents[i].Add(new Variable.VariableSource(source.ProducedCurrent).Variable);
						}
					}
				}
			}

			var result = LinearEquations.SimplifiedGaussJordanElimination(coefficients, currents);

			for(int i=0; i<nonGroundNodes; ++i)
			{
				nodes[i].Potential.Value = result[i].Evaluate().Real;
			}
		}

		#endregion

		#region Private methods		

		private List<INode> MergeNodesConnectedByWire(List<INode> nodes, List<IWire> wires)
		{
			var nodesCopy = new List<INode>(nodes);

			while(wires.Count > 0)
			{
				int selectedWireIndex = 0;
				IWire mainWire = null;
				List<INode> mainWireNodes = null;

				while (selectedWireIndex < wires.Count)
				{
					mainWire = wires[selectedWireIndex];

					mainWireNodes = nodesCopy.FindAll((node) => node.Position != null && (node.Position.Equals(mainWire.Beginning) ||
						node.Position.Equals(mainWire.Ending)));

					if (mainWireNodes.Count > 1) break;
					else ++selectedWireIndex;
				}


				var finalNode = new Node();

				for (int i = 0; i < mainWireNodes.Count(); ++i)
				{
					finalNode.Merge(mainWireNodes.ElementAt(i));
				}

				for (int i = 0; i < mainWireNodes.Count(); ++i)
				{
					nodesCopy.Remove(mainWireNodes.ElementAt(i));
				}

				var wireNetwork = new List<IWire>(mainWire.GetAllConnectedWires());

				var associatedNodes = nodesCopy.Where((node) => node.Position != null && wireNetwork.Exists((wire) =>
					node.Position.Equals(wire.Beginning) || node.Position.Equals(wire.Ending)));

				for(int i=0; i<associatedNodes.Count(); ++i)
				{
					finalNode.Merge(associatedNodes.ElementAt(i));
				}

				for (int i = 0; i < associatedNodes.Count(); ++i)
				{
					nodesCopy.Remove(associatedNodes.ElementAt(i));
				}

				nodesCopy.Add(finalNode);

				wireNetwork.ForEach((wire) => wires.Remove(wire));
				wires.RemoveAt(selectedWireIndex);
			}

			return nodesCopy;
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
				nodes.Add(GenerateNode(terminalsWithComponents));
			}

			return nodes;
		}

		/// <summary>
		/// Helper of <see cref="GenerateNodes(List{IBaseComponent})"/>. Finds all <see cref="ITerminal"/>s in the dictionary
		/// that have the same <see cref="IPlanePosition"/> as the element at index 0, creates a new <see cref="INode"/> based on them,
		/// removes those <see cref="ITerminal"/>s from the dictionary and returns the <see cref="INode"/>
		/// </summary>
		/// <param name="terminalsWithComponents"></param>
		/// <returns></returns>
		private INode GenerateNode(Dictionary<ITerminal, IBaseComponent> terminalsWithComponents)
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
	}
}