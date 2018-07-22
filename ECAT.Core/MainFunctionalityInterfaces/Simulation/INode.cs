using CSharpEnhanced.CoreClasses;
using System.Collections.Generic;

namespace ECAT.Core
{
	public interface INode
	{
		#region Properties

		/// <summary>
		/// Potential present at the node with respect to ground
		/// </summary>		
		RefWrapper<double> Potential { get; }

		/// <summary>
		/// List with all components that are connected to the <see cref="INode"/>
		/// </summary>
		List<IBaseComponent> ConnectedComponents { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Merges <paramref name="node"/> into this instance (transfers over the associated components)
		/// </summary>
		/// <param name="node"></param>
		void Merge(INode node);

		#endregion
	}
}