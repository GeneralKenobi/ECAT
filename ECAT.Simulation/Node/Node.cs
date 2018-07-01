using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Node - connects one or more components and/or wires. Used in admittance matrix and simulation
	/// </summary>
	public partial class Node
    {
		#region Constructor

		/// <summary>
		/// Default Constructor, hidden so as to force creation through <see cref=""/>
		/// </summary>
		private Node(int id)
		{
			ID = id;

			ConnectedComponents = new ReadOnlyCollection<IBaseComponent>(_ConnectedComponents);
		}

		#endregion
		
		#region Private properties

		/// <summary>
		/// Backing store for <see cref="ConnectedComponents"/>
		/// </summary>
		private List<IBaseComponent> _ConnectedComponents { get; } = new List<IBaseComponent>();

		#endregion

		#region Public properties

		/// <summary>
		/// Unique number representing the instance
		/// </summary>
		public int ID { get; }

		/// <summary>
		/// Collection of all parts connected to this node
		/// </summary>
		public ReadOnlyCollection<IBaseComponent> ConnectedComponents { get; }

		#endregion



	}
}