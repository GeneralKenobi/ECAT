using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	public partial class SimulationManager
	{
		/// <summary>
		/// Standard implementation of <see cref="ISignalInformation"/>, presents information about a signal - voltage drop or current flow		
		/// </summary>
		private class SignalInformation : Signal, ISignalInformation
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public SignalInformation() { }

			/// <summary>
			/// Copies properties from the <paramref name="signal"/>
			/// </summary>
			/// <param name="signal"></param>
			public SignalInformation(ISignal signal)
			{
				DC = signal.DC;
				ComposingPhasors = signal.ComposingPhasors;
			}

			/// <summary>
			/// Copy constructor
			/// </summary>
			public SignalInformation(ISignalInformation signalInformation)
			{
				CopyFrom(signalInformation);
			}

			#endregion

			#region Public properties

			/// <summary>
			/// True if the direction of signal was inverted (with respect to assumed directions) to present <see cref="Maximum"/> as a
			/// positive number
			/// </summary>
			public bool InvertedDirection { get; set; }

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
			/// The type of the signal
			/// </summary>
			public SignalType Type { get; set; }

			#endregion

			#region Public methods

			/// <summary>
			/// Copies all contents of <paramref name="signalInformation"/> to this object.
			/// </summary>
			/// <param name="signalInformation"></param>
			public void CopyFrom(ISignalInformation signalInformation)
			{
				if(signalInformation == null)
				{
					throw new ArgumentNullException(nameof(signalInformation));
				}

				DC = signalInformation.DC;

				ComposingPhasors = signalInformation.ComposingPhasors;

				Maximum = signalInformation.Maximum;

				Minimum = signalInformation.Minimum;

				RMS = signalInformation.RMS;

				InvertedDirection = signalInformation.InvertedDirection;

				Type = signalInformation.Type;
			}

			/// <summary>
			/// Returns a copy of this instance
			/// </summary>
			/// <returns></returns>
			ISignalInformation ISignalInformation.Copy() => Copy();

			/// <summary>
			/// Returns a copy of this instance
			/// </summary>
			/// <returns></returns>
			public SignalInformation Copy()
			{
				// Create a new instance
				var copy = new SignalInformation();

				// And copy over all properties
				copy.CopyFrom(this);

				return copy;
			}

			#endregion
		}
	}
}