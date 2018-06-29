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
	public partial class Node
    {
		/// <summary>
		/// Factory for <see cref="Node"/>s. <see cref="Node"/>s can only be created by it to make sure they are registered with
		/// </summary>
		public class Factory
		{
			#region Constructor

			/// <summary>
			/// Default Constructor
			/// </summary>
			public Factory()
			{
				Collection = new ReadOnlyCollection<Node>(_Collection);
			}

			#endregion

			#region Node construction

			/// <summary>
			/// Constructs and returns a node. The constructed node has an assigend ID and is stored in <see cref="Collection"/>
			/// </summary>
			/// <returns></returns>
			public Node Construct()
			{
				// Create a new node and increment the next id
				var node = new Node(_NextID++);

				// Add it to the collection
				_Collection.Add(node);

				// Finally return it
				return node;
			}

			#endregion

			#region Private properties

			/// <summary>
			/// ID for the next <see cref="Node"/>
			/// </summary>
			private int _NextID { get; set; } = 0;

			/// <summary>
			/// Backing store for <see cref="Collection"/>
			/// </summary>
			private List<Node> _Collection { get; }

			#endregion

			#region Public properties

			/// <summary>
			/// Collection of all created nodes
			/// </summary>
			public ReadOnlyCollection<Node> Collection { get; }

			#endregion
		}
    }
}