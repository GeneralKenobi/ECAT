using CSharpEnhanced.CoreClasses;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
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