using ECAT.Core;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Simulation
{
	public partial class SimulationManager
	{
		/// <summary>
		/// Standard implementation of <see cref="ISignalInformation"/>, presents information about a voltage drop between two nodes
		/// </summary>
		private class SignalInformation : ISignalInformation
		{
			#region Public properties

			/// <summary>
			/// True if the direction of voltage drops was inverted to present <see cref="Maximum"/> as a positive number
			/// </summary>
			public bool InvertedDirection { get; set; }

			/// <summary>
			/// DC voltage drop
			/// </summary>
			public double DC { get; set; }

			/// <summary>
			/// The maximum voltage drop that may occur
			/// </summary>
			public double Maximum { get; set; }

			/// <summary>
			/// The minimum voltage drop that may occur
			/// </summary>
			public double Minimum { get; set; }

			/// <summary>
			/// RMS value of this voltage drop
			/// </summary>
			public double RMS { get; set; }

			/// <summary>
			/// List with all AC waveforms (voltage drops) adding to the total voltage drop
			/// </summary>
			public IEnumerable<KeyValuePair<double, Complex>> ComposingACWaveforms { get; set; }

			/// <summary>
			/// The type of the voltage drop
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

				copy.ComposingACWaveforms = ComposingACWaveforms;

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