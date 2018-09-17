﻿using CSharpEnhanced.CoreInterfaces;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Signal defined in a phasor domain - composed of a DC offset and some phasors
	/// </summary>
	[RegisterAsType(typeof(IPhasorDomainSignal))]
	public class PhasorDomainSignal : IPhasorDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public PhasorDomainSignal() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public PhasorDomainSignal(IPhasorDomainSignal signal)
		{
			Copy(signal);
		}

		/// <summary>
		/// Constructor with parameter
		/// </summary>
		/// <param name="dc"></param>
		public PhasorDomainSignal(double dc)
		{
			DC = dc;
		}

		/// <summary>
		/// Constructor with parameter
		/// </summary>
		/// <param name="phasors"></param>
		public PhasorDomainSignal(IEnumerable<KeyValuePair<double, Complex>> phasors)
		{
			Phasors = phasors ?? throw new ArgumentNullException(nameof(phasors));
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="phasors">Composing phasors, exception will be thrown if null (use an empty enumeration when there are no phasors)
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignal(double dc, IEnumerable<KeyValuePair<double, Complex>> phasors)
		{
			DC = dc;
			Phasors = phasors ?? throw new ArgumentNullException(nameof(phasors));
		}

		#endregion

		#region Public properties

		/// <summary>
		/// DC component of the signal
		/// </summary>
		public double DC { get; set; }

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		public IEnumerable<KeyValuePair<double, Complex>> Phasors { get; set; } = Enumerable.Empty<KeyValuePair<double, Complex>>();

		/// <summary>
		/// Object capable of calculating characteristic values for this <see cref="ISignalData"/>
		/// </summary>
		public ISignalDataInterpreter Interpreter { get; }

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
				if (DC != 0)
				{
					result |= SignalType.DC;
				}

				// Get the number of phasors
				var phasorsCount = Phasors.Count();

				// If it's greater than 0
				if (phasorsCount > 0)
				{
					// And greater than 1, set the multi AC flag
					if (phasorsCount > 1)
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
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public PhasorDomainSignal Copy() => new PhasorDomainSignal(this);

		/// <summary>
		/// Copies internal state of <paramref name="obj"/> to this instance
		/// </summary>
		/// <param name="obj"></param>
		public void Copy(IPhasorDomainSignal obj)
		{
			DC = obj.DC;
			Phasors = obj.Phasors;
		}

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		IPhasorDomainSignal IShallowCopyTo<IPhasorDomainSignal>.Copy() => Copy();

		/// <summary>
		/// Creates a copy of the signal in reversed direction (<see cref="DC"/> and each <see cref="Phasors"/> value is negated)
		/// </summary>
		/// <returns></returns>
		public PhasorDomainSignal CopyAndNegate() => new PhasorDomainSignal(-DC, Phasors.Select((phasor) =>
			new KeyValuePair<double, Complex>(phasor.Key, -phasor.Value)));

		/// <summary>
		/// Creates a copy of the signal in reversed direction (<see cref="DC"/> and each <see cref="Phasors"/> value is negated)
		/// </summary>
		/// <returns></returns>
		IPhasorDomainSignal IPhasorDomainSignal.CopyAndNegate() => CopyAndNegate();
		
		/// <summary>
		/// Checks if average value is positive (<see cref="DC"/> is >= 0), if true returns a copy of this instance, if not returns a
		/// negated copy of this instance
		/// drops and sets the <see cref="ISignalInformation.InvertedDirection"/> flag to true
		/// </summary>
		/// <param name="info"></param>
		public PhasorDomainSignal CopyWithPositiveAverage() => DC >= 0 ? Copy() : CopyAndNegate();

		#endregion
	}
}