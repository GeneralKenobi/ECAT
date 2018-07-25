using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

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
			// Generate nodes based on their positions
			var nodes = GenerateNodes(new List<IBaseComponent>(schematic.Components));

			// Merge nodes based on the wire connections
			nodes = MergeNodesConnectedByWires(nodes, new List<IWire>(schematic.Wires));

			// Assign the potentials from the nodes to the associated terminals
			nodes.ForEach((node) => node.ConnectedTerminals.ForEach((terminal) => terminal.Potential = node.Potential));

			//NortonEquivalent(nodes);

			/////////////////////////////////////////////
			// Temporary - remove when ground is added
			// var referenceNode = nodes.Find((x) => x.ConnectedComponents.Exists((y) => y is ICurrentSource cs &&
			//	x.ConnectedTerminals.Contains(cs.TerminalA)));
			/////////////////////////////////////////////

			//nodes.Remove(referenceNode);

			var referenceNodes =
				new List<INode>(nodes.Where((node) => node.ConnectedComponents.Exists((component) => component is IGround)));

			nodes.ForEach((node) => node.Potential.Value = 0);

			nodes.RemoveAll((node) => referenceNodes.Contains(node));			

			var voltageSources = new List<IVoltageSource>(schematic.Components.Where((component) =>
				component is IVoltageSource).Select((component) => component as IVoltageSource));			

			var admittanceMatrix = ConstructDCAdmittanceMatrix(nodes, voltageSources);

			var complexMatrix = new Complex[admittanceMatrix.Item1.GetLength(0), admittanceMatrix.Item1.GetLength(1)];
			var complexFreeTerms = new Complex[admittanceMatrix.Item2.Length];

			for (int i = 0; i < complexMatrix.GetLength(0); ++i)
			{
				for (int j = 0; j < complexMatrix.GetLength(1); ++j)
				{
					complexMatrix[i, j] = admittanceMatrix.Item1[i, j].Evaluate();
				}
			}

			for (int i = 0; i < complexFreeTerms.Length; ++i)
			{
				complexFreeTerms[i] = admittanceMatrix.Item2[i].Evaluate();
			}

			try
			{
				var result = LinearEquations.SimplifiedGaussJordanElimination(complexMatrix, complexFreeTerms);

				for (int i = 0; i < nodes.Count; ++i)
				{
					nodes[i].Potential.Value += result[i].RoundTo(1e-3).Real;
				}
			}
			catch (Exception e) { }
		}

		#endregion

		#region Private methods		

		/// <summary>
		/// Fills the diagonal of a DC admittance matrix - for i-th node adds all admittances connected to it to the admittance
		/// denoted by indexes i,i
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="currents"></param>
		private void FillPassiveDCAdmittanceMatrixADiagonal(List<INode> nodes, IExpression[,] admittances)
		{
			// For each node (except node 0 - it's at 0 potential by definition so any admittence times u0 = 0 always - negligible)
			for(int i=0; i<nodes.Count; ++i)
			{
				// For each component connected to that node
				nodes[i].ConnectedComponents.ForEach((component) =>
				{
					// If the component is a two terminal and imaginary part of its admittance is zero (non-existant)
					if (component is ITwoTerminal twoTerminal && twoTerminal.Admittance.Imaginary == 0)
					{
						// Add its admittance to the matrix
						admittances[i,i] = admittances[i,i].Add(new Variable.VariableSource(twoTerminal.Admittance.Real).Variable);
					}
					// Currently components other than two terminals are not supported
					else throw new NotImplementedException();
				});
			}
		}

		/// <summary>
		/// Fills the non-diagonal entries of a DC admittance matrix - for i,j admittance subtracts from it all admittances located
		/// between node i and node j		
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		private void FillPassiveDCAdmittanceMatrixANonDiagonal(List<INode> nodes, IExpression[,] admittances)
		{
			for (int i = 0; i < nodes.Count; ++i)
			{
				for (int j = 0; j < i; ++j)
				{
					// Find all components located between node i and node j
					var admittancesBetweenNodesij =
						new List<IBaseComponent>(nodes[i].ConnectedComponents.Intersect(nodes[j].ConnectedComponents));

					// For each of them
					admittancesBetweenNodesij.ForEach((component) =>
					{
						// If the component is a two terminal and imaginary part of its admittance is zero (non-existant)
						if (component is ITwoTerminal twoTerminal && twoTerminal.Admittance.Imaginary == 0)
						{
							// Subtract its admittance to the matrix
							admittances[i, j] = admittances[i, j].Subtract(
								new Variable.VariableSource(twoTerminal.Admittance.Real).Variable);

							// And do the same to the entry j,i - admittances between node i,j are identical to admittances
							// between nodes j,i
							admittances[j, i] = admittances[j, i].Subtract(
								new Variable.VariableSource(twoTerminal.Admittance.Real).Variable);
						}
						// Currently components other than two terminals are not supported
						else throw new NotImplementedException();
					});
				}
			}
		}

		private void FillPassiveDCAdmittanceBMatrix(List<INode> nodes, IExpression[,] admittances,
			List<IVoltageSource> voltageSources)
		{
			for(int i=0; i<voltageSources.Count; ++i)
			{
				if(nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB)))
				{					
					admittances[nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB)), nodes.Count + i] = Variable.One;
				}

				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA)))
				{
					admittances[nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA)), nodes.Count + i] = Variable.NegativeOne;
				}
			}
		}

		private void FillPassiveDCAdmittanceCMatrix(List<INode> nodes, IExpression[,] admittances,
			List<IVoltageSource> voltageSources)
		{
			for (int i = 0; i < voltageSources.Count; ++i)
			{
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB)))
				{
					admittances[nodes.Count + i, nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB))] = Variable.One;
				}

				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA)))
				{
					admittances[nodes.Count + i, nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA))] = Variable.NegativeOne;
				}
			}
		}

		private void FillPassiveDCAdmittanceDMatrix(List<INode> nodes, IExpression[,] admittances)
		{
			// TODO: Fill accordingly when dependent sources are added
			for (int i = nodes.Count; i < admittances.GetLength(0); ++i)
			{
				for (int j = nodes.Count; j < admittances.GetLength(1); ++j)
				{
					admittances[i, j] = Variable.Zero;
				}
			}
		}


		private void AddCurrents(List<INode> nodes, IExpression[] currents)
		{
			for (int i = 0; i < nodes.Count; ++i)
			{
				nodes[i].ConnectedComponents.ForEach((component) =>
				{
					if (component is ICurrentSource source)
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
				});
			}
		}

		private void AddVoltages(List<INode> nodes, IExpression[] currents, List<IVoltageSource> voltageSources)
		{
			for (int i = 0; i < voltageSources.Count; ++i)
			{
				currents[nodes.Count + i] = new Variable.VariableSource(voltageSources[i].ProducedVoltage).Variable;
			}
		}

		private Tuple<IExpression[,], IExpression[]> ConstructDCAdmittanceMatrix(List<INode> nodes,
			List<IVoltageSource> independentVoltageSources)
		{
			// The size of the system
			int size = nodes.Count + independentVoltageSources.Count;

			// Create arrays
			var admittances = ArrayHelpers.CreateAndInitialize<IExpression>(Variable.Zero, size, size);
			var currents = ArrayHelpers.CreateAndInitialize<IExpression>(Variable.Zero, size);

			// TODO: When added, handle the active components

			// Fill the passive admittance matrix
			FillPassiveDCAdmittanceMatrixADiagonal(nodes, admittances);
			FillPassiveDCAdmittanceMatrixANonDiagonal(nodes, admittances);
			FillPassiveDCAdmittanceBMatrix(nodes, admittances, independentVoltageSources);
			FillPassiveDCAdmittanceCMatrix(nodes, admittances, independentVoltageSources);
			FillPassiveDCAdmittanceDMatrix(nodes, admittances);

			

			AddCurrents(nodes, currents);
			AddVoltages(nodes, currents, independentVoltageSources);

			return new Tuple<IExpression[,], IExpression[]>(admittances, currents);
		}

		/// <summary>
		/// Helper of <see cref="MergeNodesConnectedByWires(List{INode}, List{IWire})"/>, goes through the passed list of wires and,
		/// if it finds a wire that connects to at least one component, creates an <see cref="INode"/> that will be merged with nodes
		/// at both ends of the selected wire. Removes the merged wires from the passed node list, removes the wire from the passed
		/// wire list. Modifies <paramref name="nodes"/> and <paramref name="wires"/>, returns true if a wire connected to at least
		/// one component was found (at least one end of the wire connects to a component), false otherwise.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="wires"></param>
		/// <param name="createdNode"></param>
		/// <param name="selectedWire"></param>
		/// <returns></returns>
		private bool InitialWireMerge(List<INode> nodes, List<IWire> wires, out INode createdNode, out IWire selectedWire)
		{
			// Initially assign null to out parameters
			createdNode = null;
			selectedWire = null;

			// A list for nodes of the selected wire that actually has nodes (at least one end of the wire connects to a component)
			List<INode> mainWireNodes = null;

			// Go through each wire to find the first wire that has nodes
			foreach(var wire in wires)
			{
				// Get all nodes from the list that are positioned either at the beginning or ending of the wire
				mainWireNodes = nodes.FindAll((node) => node.Position != null && (node.Position.Equals(wire.Beginning) ||
					node.Position.Equals(wire.Ending)));

				// If there was at least one
				if (mainWireNodes.Count > 1)
				{
					// Assign the selected wire and finish the loop
					selectedWire = wire;
					break;
				}
			}

			// If the selected wire was not assigned then it means the remaining wires don't connect with any components - nothing
			// else to do
			if(selectedWire == null)
			{
				return false;
			}

			// If the method got here then it means that it found a wire which connects to at least  one component
			// Create a node for it
			createdNode = new Node();

			// And a copy to use in lambdas
			var copyForLambdas = createdNode;

			// Merge the nodes into the new Node
			mainWireNodes.ForEach((node) => copyForLambdas.Merge(node));

			// Remove them from the list of nodes
			mainWireNodes.ForEach((node) => nodes.Remove(node));

			// Remove the wire from the list of wires
			wires.Remove(selectedWire);

			// Return success
			return true;
		}

		/// <summary>
		/// Merges all nodes that are connected by a wire network (modifies both lists)
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="wires"></param>
		/// <returns></returns>
		private List<INode> MergeNodesConnectedByWires(List<INode> nodes, List<IWire> wires)
		{
			// Keep going until all wires were considered
			while(wires.Count > 0)
			{
				// If the initial merge didn't succeed, stop the loop there is no more merging to be done
				if(!InitialWireMerge(nodes, wires, out INode finalNode, out IWire selectedWire))
				{
					break;
				}

				// If the method got here then it means that a suitable wire was found, get a list of all wires that are connected
				// to it
				var wireNetwork = new List<IWire>(selectedWire.GetAllConnectedWires());

				// Get all nodes that are associated with the ends of those wires (at this point nodes may have null positions which
				// means those nodes are no longer position-based, so check for that)
				var associatedNodes = new List<INode>(nodes.Where((node) => node.Position != null && wireNetwork.Exists((wire) =>
					node.Position.Equals(wire.Beginning) || node.Position.Equals(wire.Ending))));

				// Merge all found nodes
				associatedNodes.ForEach((node) => finalNode.Merge(node));

				// Remove them from the collection
				associatedNodes.ForEach((node) => nodes.Remove(node));

				// Add the node that remained after merging to the collection
				nodes.Add(finalNode);

				// Remove all wires that were handled in the process
				wireNetwork.ForEach((wire) => wires.Remove(wire));
			}

			return nodes;
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