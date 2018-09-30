using CSharpEnhanced.CoreInterfaces;
using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	/// <summary>
	/// Standard implementation of <see cref="ISignalInformation"/>, presents information about a signal - voltage drop or current flow		
	/// </summary>
	public class SignalInformation : ISignalInformation
	{
		#region Constructors

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="data"></param>
		/// <param name="description"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public SignalInformation(ISignalData data, ISignalDescription description)
		{
			Data = data ?? throw new ArgumentNullException(nameof(data));
			Description = description ?? throw new ArgumentNullException(nameof(description));
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public SignalInformation(ISignalInformation source)
		{
			Copy(source ?? throw new ArgumentNullException(nameof(source)));
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
		/// Description (meaning) of this <see cref="ISignalInformation"/>
		/// </summary>
		public ISignalDescription Description { get; private set; }

		/// <summary>
		/// The maximum signal value that may occur
		/// </summary>
		public double Maximum { get; private set; }

		/// <summary>
		/// The minimum signal that may occur
		/// </summary>
		public double Minimum { get; private set; }

		/// <summary>
		/// RMS value of this signal
		/// </summary>
		public double RMS { get; private set; }

		/// <summary>
		/// The average value of the signal
		/// </summary>
		public double Average { get; private set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Updates properties based on new <see cref="Data"/>
		/// </summary>
		private void DataChanged()
		{
			Maximum = Data.Interpreter.Maximum();
			Minimum = Data.Interpreter.Minimum();
			RMS = Data.Interpreter.RMS();
			Average = Data.Interpreter.Average();
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

			Description = signalInformation.Description;

			Maximum = signalInformation.Maximum;

			Minimum = signalInformation.Minimum;

			RMS = signalInformation.RMS;
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