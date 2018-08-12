using CSharpEnhanced.CoreClasses;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Node - connects one or more components and/or wires. Used in admittance matrix and simulation
	/// </summary>
	public partial class Node : INode
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public Node() { }

		/// <summary>
		/// Constructor taking an <see cref="IPlanePosition"/>
		/// </summary>
		/// <param name="position"></param>
		public Node(IPlanePosition position)
		{
			Position = position;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The position of the node
		/// </summary>
		public IPlanePosition Position { get; }

		/// <summary>
		/// AC potentials present at the node with respect to ground. Item1 (double) refers
		/// to the frequency of the source generating the potential and Item2 (Complex) to the value of the potential.
		/// </summary>
		public ICollection<Tuple<double, Complex>> ACPotentials { get; } = new List<Tuple<double, Complex>>();

		/// <summary>
		/// The DC potential of the node with respect to ground
		/// </summary>
		public RefWrapper<double> DCPotential { get; } = new RefWrapper<double>();

		/// <summary>
		/// List with all components that are connected to the <see cref="INode"/>
		/// </summary>
		public List<IBaseComponent> ConnectedComponents { get; set; } = new List<IBaseComponent>();

		/// <summary>
		/// List of all terminals that are associated with this <see cref="INode"/>
		/// </summary>
		public List<ITerminal> ConnectedTerminals { get; set; } = new List<ITerminal>();

		#endregion

		#region Public Methods

		/// <summary>
		/// Merges <paramref name="node"/> into this instance (transfers over the associated components and terinals)
		/// </summary>
		/// <param name="node"></param>
		public void Merge(INode node)
		{
			// If the nodes are distinct
			if (node != this)
			{
				// Transfer over connected components and terminals to this
				ConnectedComponents.AddRange(node.ConnectedComponents);
				ConnectedTerminals.AddRange(node.ConnectedTerminals);

				// Remove them from the other node
				node.ConnectedComponents.Clear();
				node.ConnectedTerminals.Clear();
			}
		}

		#endregion
	}
}