using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Node - connects one or more components and/or wires
	/// </summary>
	public class Node
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Node()
		{
			ConnectedComponents = new ReadOnlyCollection<BaseComponent>(_ConnectedComponents);
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="ConnectedComponents"/>
		/// </summary>
		private List<BaseComponent> _ConnectedComponents { get; } = new List<BaseComponent>();

		#endregion

		#region Public properties

		/// <summary>
		/// Unique number representing the instance
		/// </summary>
		public int ID { get; }

		/// <summary>
		/// Collection of all parts connected to this node
		/// </summary>
		public ReadOnlyCollection<BaseComponent> ConnectedComponents { get; }

		#endregion



	}
}