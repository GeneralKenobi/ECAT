using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	partial class SimulationResultsBias
	{
		/// <summary>
		/// Class used to store potential on a node after bias simulation
		/// </summary>
		[RegisterAsType(typeof(INodePotentialBias))]
		private class NodePotentialBias : INodePotentialBias
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="dc"></param>
			/// <param name="phasors"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public NodePotentialBias(double dc, IDictionary<double, Complex> phasors)
			{
				DC = dc;
				Phasors = phasors ?? throw new ArgumentNullException(nameof(phasors));
			}

			#endregion

			#region Public properties

			/// <summary>
			/// AC potentials present at the node with respect to ground. Item1 (double) refers
			/// to the frequency of the source generating the potential and Item2 (Complex) to the value of the potential.
			/// </summary>		
			public IDictionary<double, Complex> Phasors { get; } = new Dictionary<double, Complex>();

			/// <summary>
			/// The DC potential of the node with respect to ground
			/// </summary>
			public double DC { get; }

			#endregion
		}
	}
}