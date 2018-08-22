using ECAT.Core;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	public partial class SimulationManager
	{
		/// <summary>
		/// Standard implementation of <see cref="ISignalInformation"/>, presents information about a signal - voltage drop or current flow		
		/// </summary>
		private class SignalInformation : ISignalInformation
		{
			#region Public properties

			/// <summary>
			/// True if the direction of signal was inverted (with respect to assumed directions) to present <see cref="Maximum"/> as a
			/// positive number
			/// </summary>
			public bool InvertedDirection { get; set; }

			/// <summary>
			/// DC component
			/// </summary>
			public double DC { get; set; }

			/// <summary>
			/// The maximum signal value that may occur
			/// </summary>
			public double Maximum { get; set; }

			/// <summary>
			/// The minimum signal that may occur
			/// </summary>
			public double Minimum { get; set; }

			/// <summary>
			/// RMS value of this signal
			/// </summary>
			public double RMS { get; set; }

			/// <summary>
			/// List with all phasors adding to the total voltage drop
			/// </summary>
			public IEnumerable<KeyValuePair<double, Complex>> ComposingPhasors { get; set; } =
				Enumerable.Empty<KeyValuePair<double, Complex>>();

			/// <summary>
			/// The type of the signal
			/// </summary>
			public SignalType Type { get; set; }

			#endregion

			#region Public methods

			/// <summary>
			/// Returns a copy of this instance
			/// </summary>
			/// <returns></returns>
			public SignalInformation Copy()
			{
				// Create a new instance and copy over all properties
				var copy = new SignalInformation();

				copy.DC = DC;

				copy.ComposingPhasors = ComposingPhasors;

				copy.Maximum = Maximum;

				copy.Minimum = Minimum;

				copy.RMS = RMS;

				copy.InvertedDirection = InvertedDirection;

				copy.Type = Type;

				return copy;
			}

			#endregion
		}
	}
}