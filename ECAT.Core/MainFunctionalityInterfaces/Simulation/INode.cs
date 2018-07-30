using CSharpEnhanced.CoreClasses;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for nodes. Node is a class that connects <see cref="IBaseComponent"/>s and is used as the basis of simulation
	/// (nodal analysis calculates potentials at all nodes)
	/// </summary>
	public interface INode
	{
		#region Properties

		/// <summary>
		/// The position of the node
		/// </summary>
		IPlanePosition Position { get; }

		/// <summary>
		/// Potential present at the node with respect to ground
		/// </summary>		
		RefWrapperPropertyChanged<double> Potential { get; }

		/// <summary>
		/// List with all components that are connected to the <see cref="INode"/>
		/// </summary>
		List<IBaseComponent> ConnectedComponents { get; set; }

		/// <summary>
		/// List of all terminals that are associated with this <see cref="INode"/>
		/// </summary>
		List<ITerminal> ConnectedTerminals { get; set; }
		
		#endregion

		#region Methods

		/// <summary>
		/// Merges <paramref name="node"/> into this instance (transfers over the associated components and terminals)
		/// </summary>
		/// <param name="node"></param>
		void Merge(INode node);

		#endregion
	}
}