using CSharpEnhanced.CoreInterfaces;
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
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public Signal() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public Signal(ISignal signal)
		{
			Copy(signal);
		}

		#endregion

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

		#region Public methods

		/// <summary>
		/// Copies contents of <paramref name="obj"/> into this object
		/// </summary>
		/// <param name="obj"></param>
		public void Copy(ISignal obj)
		{
			DC = obj.DC;
			ComposingPhasors = obj.ComposingPhasors;
		}

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public Signal CopySignal() => new Signal(this);

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		ISignal IDeepCopyTo<ISignal>.Copy() => CopySignal();

		#endregion
	}
}