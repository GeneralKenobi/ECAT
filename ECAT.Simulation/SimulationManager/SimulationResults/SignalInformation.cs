using CSharpEnhanced.CoreInterfaces;
using ECAT.Core;
using System;
using System.Linq;

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
			public SignalInformation(ISignal signal) : base(signal) { }			

			/// <summary>
			/// Copy constructor
			/// </summary>
			public SignalInformation(ISignalInformation signalInformation)
			{
				Copy(signalInformation);
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
			public SignalType Type
			{
				get
				{
					// Create an enumeration equal to 0
					var result = SignalType.Empty;
					
					// Check for DC, if present set the flag
					if(DC != 0)
					{
						result |= SignalType.DC;
					}

					// Get the number of phasors
					var phasorsCount = ComposingPhasors.Count();

					// If it's greater than 0
					if(phasorsCount > 0)
					{
						// And greater than 1, set the multi AC flag
						if(phasorsCount > 1)
						{
							result |= SignalType.MultipleAC;
						}
						// Otherwise just set the single AC flag
						else
						{
							result |= SignalType.SingleAC;
						}						
					}

					return result;
				}
			}

			#endregion

			#region Public methods

			/// <summary>
			/// Copies all contents of <paramref name="signalInformation"/> to this object.
			/// </summary>
			/// <param name="signalInformation"></param>
			public void Copy(ISignalInformation signalInformation)
			{
				if(signalInformation == null)
				{
					throw new ArgumentNullException(nameof(signalInformation));
				}

				// Copy contents from the base class
				base.Copy(signalInformation);

				// Now copy own properties

				Maximum = signalInformation.Maximum;

				Minimum = signalInformation.Minimum;

				RMS = signalInformation.RMS;

				InvertedDirection = signalInformation.InvertedDirection;
			}

			/// <summary>
			/// Returns a copy of this instance
			/// </summary>
			/// <returns></returns>
			ISignalInformation IDeepCopyTo<ISignalInformation>.Copy() => CopySignalInformation();

			/// <summary>
			/// Returns a copy of this instance
			/// </summary>
			/// <returns></returns>
			public SignalInformation CopySignalInformation() =>	new SignalInformation(this);

			#endregion
		}
	}
}