using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections.Generic;
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
			// Generate nodes using helper class
			var nodes = NodeGenerator.Generate(schematic);

			// Assign the potentials from the nodes to the associated terminals
			nodes.ForEach((node) => node.ConnectedTerminals.ForEach((terminal) => terminal.Potential = node.Potential));

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

		

		#endregion
	}
}