using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Class used to store potential on a node after bias simulation
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration(new Type[] { typeof(double), typeof(IDictionary<double, Complex>)}, "DC", "AC Phasors")]
	public interface INodePotentialBias
    {
		#region Properties

		/// <summary>
		/// AC potentials present at the node with respect to ground. Item1 (double) refers
		/// to the frequency of the source generating the potential and Item2 (Complex) to the value of the potential.
		/// </summary>		
		IDictionary<double, Complex> Phasors { get; }

		/// <summary>
		/// The DC potential of the node with respect to ground
		/// </summary>
		double DC { get; }

		#endregion
	}
}