using CSharpEnhanced.CoreClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Node - connects one or more components and/or wires. Used in admittance matrix and simulation
	/// </summary>
	public partial class Node : INode
    {
		#region Constructor

		public Node() { }

		/// <summary>
		/// Default Constructor, hidden so as to force creation through <see cref=""/>
		/// </summary>
		private Node(int id)
		{
			
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Potential present at the node with respect to ground
		/// </summary>
		public RefWrapper<double> Potential { get; }

		/// <summary>
		/// List with all components that are connected to the <see cref="INode"/>
		/// </summary>
		public List<IBaseComponent> ConnectedComponents { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Merges <paramref name="node"/> into this instance (transfers over the associated components)
		/// </summary>
		/// <param name="node"></param>
		public void Merge(INode node)
		{
			ConnectedComponents.AddRange(node.ConnectedComponents);

			node.ConnectedComponents.Clear();
		}

		#endregion
	}
}