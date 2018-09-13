using CSharpEnhanced.CoreInterfaces;
using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	public partial class SimulationManager
	{
		/// <summary>
		/// Standard implementation of <see cref="ISignalInformation"/>, presents information about a signal - voltage drop or current flow		
		/// </summary>
		private class SignalInformation : ISignalInformation
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public SignalInformation() { }

			/// <summary>
			/// Constructor with parameter
			/// </summary>
			public SignalInformation(ISignalData data)
			{
				Data = data;
			}

			/// <summary>
			/// Copy constructor
			/// </summary>
			public SignalInformation(ISignalInformation source)
			{
				Copy(source);
			}

			#endregion

			#region Private members

			/// <summary>
			/// Backing store for <see cref="Data"/>
			/// </summary>
			ISignalData mData;

			#endregion

			#region Public properties

			/// <summary>
			/// Raw data of the signal
			/// </summary>
			public ISignalData Data
			{
				get => mData;
				private set
				{
					if(mData != value)
					{
						mData = value;
						DataChanged();
					}
				}
			}

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

			#endregion

			#region Private methods

			private void DataChanged()
			{
				if (Data != null)
				{
					Maximum = Data.Interpreter.Maximum();
					Minimum = Data.Interpreter.Minimum();
					RMS = Data.Interpreter.RMS();
				}
				else
				{
					Maximum = 0;
					Minimum = 0;
					RMS = 0;
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

				Data = signalInformation.Data;

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