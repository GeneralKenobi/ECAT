﻿using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System.Collections.Generic;
using System.Linq;


/////////////////////////////////////////////////////////////////////////////////////////////////
//																							   //
// Note: The rules according to which the DC admittance matrix is created are described in the //
// DCAdmittanceMatrixConstructionRules.txt text file located in this folder.				   //
//																							   //
/////////////////////////////////////////////////////////////////////////////////////////////////


namespace ECAT.Simulation
{
	/// <summary>
	/// Helper of <see cref="SimulationManager"/>, constructs a DC admittance matrix from the given collection of Nodes and
	/// independent voltage sources. DC matrix treats all reactive components as open circuits, all AC sources as short circuits.
	/// The matrix 
	/// </summary>
	public class DCAdmittanceMatrix : AdmittanceMatrix
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected DCAdmittanceMatrix(IExpression[,] aMatrix, IExpression[] zMatrix, List<INode> nodes, List<IVoltageSource> sources) :
			base(aMatrix, zMatrix, nodes, sources) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Solves the matrix for the parameter values present at the moment of calling,
		/// updates the values of nodes and sources currents
		/// </summary>
		public override void Solve()
		{
			// Evaluate the matrices and solve the system
			var result = LinearEquations.SimplifiedGaussJordanElimination(_A.Evaluate(), _Z.Evaluate());

			// Assign the node potentials (entries from 0 to the number of nodes - 1)
			for(int i=0; i<_NodePotentials.Count; ++i)
			{
				_NodePotentials[i].Value = result[i].Real;
			}

			// Assign the currents through voltage sources (the remaining entries of the results)
			for (int i = _NodePotentials.Count; i < _NodePotentials.Count; ++i)
			{
				_VoltageSourcesCurrents[i - _NodePotentials.Count].Value = result[i].Real;
			}
		}

		#endregion

		#region Private static methods

		/// <summary>
		/// Fills the diagonal of a DC admittance matrix - for i-th node adds all admittances connected to it to the admittance
		/// denoted by indexes i,i
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="currents"></param>
		private static void FillPassiveGMatrixDiagonal(List<INode> nodes, IExpression[,] admittances)
		{
			// For each node
			for (int i = 0; i < nodes.Count; ++i)
			{
				// For each component connected to that node
				nodes[i].ConnectedComponents.ForEach((component) =>
				{
					// If the component is a two terminal and imaginary part of its admittance is zero (non-existant)
					if (component is ITwoTerminal twoTerminal && twoTerminal.AdmittanceVar.Value.Imaginary == 0)
					{
						// Add its admittance to the matrix
						admittances[i, i] = admittances[i, i].Add(twoTerminal.AdmittanceVar);
					}					
				});
			}
		}

		/// <summary>
		/// Fills the non-diagonal entries of a DC admittance matrix - for i,j admittance subtracts from it all admittances located
		/// between node i and node j		
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		private static void FillPassiveGMatrixNonDiagonal(List<INode> nodes, IExpression[,] admittances)
		{
			// For each node
			for (int i = 0; i < nodes.Count; ++i)
			{
				// For each node it's pair with (because of that matrix G is symmetrical so it's only necessary to fill the
				// part below main diagonal and copy the operation to the corresponding entry above the main diagonal
				for (int j = 0; j < i; ++j)
				{
					// Find all components located between node i and node j
					var admittancesBetweenNodesij =
						new List<IBaseComponent>(nodes[i].ConnectedComponents.Intersect(nodes[j].ConnectedComponents));

					// For each of them
					admittancesBetweenNodesij.ForEach((component) =>
					{
						// If the component is a two terminal and imaginary part of its admittance is zero (non-existant)
						if (component is ITwoTerminal twoTerminal && twoTerminal.AdmittanceVar.Value.Imaginary == 0)
						{
							// Subtract its admittance to the matrix
							admittances[i, j] = admittances[i, j].Subtract(twoTerminal.AdmittanceVar);

							// And do the same to the entry j,i - admittances between node i,j are identical to admittances
							// between nodes j,i
							admittances[j, i] = admittances[j, i].Subtract(twoTerminal.AdmittanceVar);
						}
					});
				}
			}
		}

		/// <summary>
		/// Fills the B matrix, rules are described in a separate text file
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="voltageSources"></param>
		private static void FillPassiveBMatrix(List<INode> nodes, IExpression[,] admittances,
			List<IVoltageSource> voltageSources, List<IOpAmp> opAmps)
		{
			// Consider voltage sources, start column is the one on the right of G matrix (whose size is the number of nodes)
			FillBMatrixBasedOnVoltageSources(nodes, admittances, voltageSources, nodes.Count);

			// Consider op-amps, start column is after column related to nodes and voltage sources
			FillBMatrixBasedOnOpAmps(nodes, admittances, opAmps, nodes.Count + voltageSources.Count);
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveBMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>, fills the
		/// B part of admittance matrix with -1, 0 or 1 based on voltage sources present in the circuit
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="voltageSources"></param>
		/// <param name="startColumn"></param>
		private static void FillBMatrixBasedOnVoltageSources(List<INode> nodes, IExpression[,] admittances,
			List<IVoltageSource> voltageSources, int startColumn)
		{
			// For every voltage source
			for (int i = 0; i < voltageSources.Count; ++i)
			{
				// If there exists a node to which TerminalB is connected (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB)))
				{
					// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
					// with 1 (positive terminal)
					admittances[nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB)),
						startColumn + i] = Variable.One;
				}

				// If there exists a node to which TerminalA is connected (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA)))
				{
					// Fill the entry in the row corresponding to the node and column corresponding to the source (plus start column)
					// with -1 (negative terminal)
					admittances[nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA)),
						startColumn + i] = Variable.NegativeOne;
				}
			}
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveBMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>, fills the
		/// B part of admittance matrix with -1, 0 or 1 based on op-amps present in the circuit (treats their output as a voltage
		/// source)
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="voltageSources"></param>
		/// <param name="startColumn"></param>
		private static void FillBMatrixBasedOnOpAmps(List<INode> nodes, IExpression[,] admittances, List<IOpAmp> opAmps,
			int startColumn)
		{
			// For every op-amp
			for (int i = 0; i < opAmps.Count; ++i)
			{
				// If there exists a node to which TerminalC (op-amps output) is connected
				// (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(opAmps[i].TerminalC)))
				{
					// Fill the entry in the row corresponding to the node and column corresponding to the source
					// (plus start column) with 1 (positive terminal)
					admittances[nodes.FindIndex((node) => node.ConnectedTerminals.Contains(opAmps[i].TerminalC)), startColumn + i] =
						Variable.One;
				}
			}
		}

		/// <summary>
		/// Fills the C matrix, rules are described in a separate text file
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="voltageSources"></param>
		private static void FillPassiveCMatrix(List<INode> nodes, IExpression[,] admittances,
			List<IVoltageSource> voltageSources, List<IOpAmp> opAmps)
		{
			// All entries in C are 0 by default

			// Consider voltage sources, the work area in admittance matrix start on the left, below G matrix (size equal to
			// number of nodes)
			FillCMatrixBasedOnVoltageSources(nodes, admittances, voltageSources, nodes.Count);

			// Consider op-amps , the work area in admittance matrix start on the left, below G matrix and rows associated with
			// voltage sources (number of nodes plus number of voltage sources)
			FillCMatrixBasedOnOpAmps(nodes, admittances, opAmps, nodes.Count + voltageSources.Count);
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveCMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>,
		/// fills C part of admittance matrix with -1, 0 or 1 based on voltage sources present in the circuit
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="voltageSources"></param>
		/// <param name="startRow"></param>
		private static void FillCMatrixBasedOnVoltageSources(List<INode> nodes, IExpression[,] admittances,
			List<IVoltageSource> voltageSources, int startRow)
		{
			// For every voltage source
			for (int i = 0; i < voltageSources.Count; ++i)
			{
				// If there exists a node to which TerminalB is connected (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB)))
				{
					// Fill the entry in the row corresponding to the source (plus starting row)
					// and column corresponding to the node with 1 (positive terminal)
					admittances[startRow + i,
						nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalB))] = Variable.One;
				}

				// If there exists a node to which TerminalA is connected (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA)))
				{
					// Fill the entry in the row corresponding to the source (plus starting row)
					// and column corresponding to the node with -1 (negative terminal)
					admittances[startRow + i,
						nodes.FindIndex((node) => node.ConnectedTerminals.Contains(voltageSources[i].TerminalA))] = Variable.NegativeOne;
				}
			}
		}

		/// <summary>
		/// Helper of <see cref="FillPassiveCMatrix(List{INode}, IExpression[,], List{IVoltageSource}, List{IOpAmp})"/>,
		/// fills C part of admittance matrix with -1, 0 or 1 based on op-amps present in the circuit
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		/// <param name="opAmps"></param>
		/// <param name="startRow"></param>
		private static void FillCMatrixBasedOnOpAmps(List<INode> nodes, IExpression[,] admittances,	List<IOpAmp> opAmps, int startRow)
		{
			// For every op-amp
			for (int i = 0; i < opAmps.Count; ++i)
			{
				// If there exists a node to which TerminalA (non-inverting input) is connected
				// (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(opAmps[i].TerminalA)))
				{
					// Fill the entry in the row corresponding to the op-amp (plus starting row)
					// and column corresponding to the node with 1 (positive terminal)
					admittances[i + startRow,
						nodes.FindIndex((node) => node.ConnectedTerminals.Contains(opAmps[i].TerminalA))] = Variable.One;
				}

				// If there exists a node to which TerminalB (inverting input) is connected
				// (it's possible it may not exist due to removed ground node)
				if (nodes.Exists((node) => node.ConnectedTerminals.Contains(opAmps[i].TerminalB)))
				{
					// Fill the entry in the row corresponding to the op-amp (plus starting row)
					// and column corresponding to the node with -1 (positive terminal)
					admittances[i + startRow,
						nodes.FindIndex((node) => node.ConnectedTerminals.Contains(opAmps[i].TerminalB))] = Variable.NegativeOne;
				}
			}
		}

		/// <summary>
		/// Fills the D matrix according to rules described in a separate text file
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="admittances"></param>
		private static void FillPassiveDMatrix(List<INode> nodes, IExpression[,] admittances)
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

		/// <summary>
		/// Fills the currents in the Z matrix (the top part)
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="currents"></param>
		private static void FillZMatrixCurrents(List<INode> nodes, IExpression[] currents)
		{
			// For every node
			for (int i = 0; i < nodes.Count; ++i)
			{
				// Go through each connected component
				nodes[i].ConnectedComponents.ForEach((component) =>
				{
					// If it's a current source
					if (component is ICurrentSource source)
					{
						// If the positive terminal is connected, add the current
						if (nodes[i].ConnectedTerminals.Contains(source.TerminalB))
						{
							currents[i] = currents[i].Add(source.ProducedCurrentVar);
						}
						// If the negative terminal is connected, subtract the current
						else
						{
							currents[i] = currents[i].Subtract(source.ProducedCurrentVar);
						}
					}
				});
			}
		}

		/// <summary>
		/// Fills the voltages in the Z matrix (the bottom part)
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="currents"></param>
		/// <param name="voltageSources"></param>
		private static void FillZMatrixVoltages(List<INode> nodes, IExpression[] currents, List<IVoltageSource> voltageSources)
		{
			// For each voltage source
			for (int i = 0; i < voltageSources.Count; ++i)
			{
				// Add its voltge to the i-th entry plus the number of nodes (currents present above in the matrix)
				currents[nodes.Count + i] = voltageSources[i].ProducedVoltageVar;
			}
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Constructs a <see cref="DCAdmittanceMatrix"/> from the given nodes and given independent <see cref="IVoltageSource"/>s.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="independentVS">Collection of all independent <see cref="IVoltageSource"/>s in the schematic</param>
		/// <returns></returns>
		public static DCAdmittanceMatrix Construct(List<INode> nodes, List<IVoltageSource> independentVS, List<IOpAmp> opAmps)
		{
			// The size of the system
			int size = nodes.Count + independentVS.Count + opAmps.Count;

			// Create the arrays
			var aMatrix = ArrayHelpers.CreateAndInitialize<IExpression>(Variable.Zero, size, size);
			var zMatrix = ArrayHelpers.CreateAndInitialize<IExpression>(Variable.Zero, size);

			// TODO: When added, handle the active components

			// Fill all parts of the A matrix
			FillPassiveGMatrixDiagonal(nodes, aMatrix);
			FillPassiveGMatrixNonDiagonal(nodes, aMatrix);

			FillPassiveBMatrix(nodes, aMatrix, independentVS, opAmps);
			FillPassiveCMatrix(nodes, aMatrix, independentVS, opAmps);
			FillPassiveDMatrix(nodes, aMatrix);

			// Fill all parts of Z matrix
			FillZMatrixCurrents(nodes, zMatrix);
			FillZMatrixVoltages(nodes, zMatrix, independentVS);

			// Construct a new instance and return it
			return new DCAdmittanceMatrix(aMatrix, zMatrix, nodes, independentVS);
		}

		#endregion
	}
}