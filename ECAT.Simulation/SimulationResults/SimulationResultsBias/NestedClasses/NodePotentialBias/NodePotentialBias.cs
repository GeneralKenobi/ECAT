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
		private class NodePotentialBias : INodePotentialBiasControl
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
				_Phasors = new Dictionary<double, Complex>(phasors ?? throw new ArgumentNullException(nameof(phasors)));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Backing store for <see cref="Phasors"/>
			/// </summary>
			private Dictionary<double, Complex> _Phasors { get; } = new Dictionary<double, Complex>();

			#endregion

			#region Public properties

			/// <summary>
			/// AC potentials present at the node with respect to ground. Item1 (double) refers
			/// to the frequency of the source generating the potential and Item2 (Complex) to the value of the potential.
			/// </summary>		
			public IDictionary<double, Complex> Phasors => _Phasors;

			/// <summary>
			/// The DC potential of the node with respect to ground
			/// </summary>
			public double DC { get; private set; }

			#endregion

			#region Public methods

			/// <summary>
			/// Used to set the value of <see cref="INodePotentialBias.DC"/> property
			/// </summary>
			/// <param name="dc"></param>
			public void SetDC(double dc) => DC = dc;

			/// <summary>
			/// Used to add <see cref="KeyValuePair{TKey, TValue}"/> to <see cref="INodePotentialBias.Phasors"/>
			/// </summary>
			/// <param name="frequency"></param>
			/// <param name="value"></param>
			public void AddPhasor(double frequency, Complex value) => _Phasors.Add(frequency, value);

			#endregion
		}
	}
}