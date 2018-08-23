using CSharpEnhanced.CoreInterfaces;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes defining a signal - voltage drop or current flow
	/// </summary>
	public interface ISignal : IDeepCopy<ISignal>
	{
		#region Properties

		/// <summary>
		/// DC component of the signal
		/// </summary>
		double DC { get; }

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		IEnumerable<KeyValuePair<double, Complex>> ComposingPhasors { get; }

		#endregion
	}
}