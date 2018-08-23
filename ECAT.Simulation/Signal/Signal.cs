using ECAT.Core;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Standard implementation of <see cref="ISignal"/>, represents a single signal that may be measured in a circuit
	/// </summary>
	public class Signal : ISignal
    {
		#region Public properties

		/// <summary>
		/// DC component
		/// </summary>
		public double DC { get; set; }

		/// <summary>
		/// List with all phasors adding to the total signal
		/// </summary>
		public IEnumerable<KeyValuePair<double, Complex>> ComposingPhasors { get; set; } =
			Enumerable.Empty<KeyValuePair<double, Complex>>();

		#endregion
	}
}