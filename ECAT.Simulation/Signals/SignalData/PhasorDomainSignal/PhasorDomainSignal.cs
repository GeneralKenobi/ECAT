using CSharpEnhanced.CoreInterfaces;
using CSharpEnhanced.Helpers;
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
	public partial class PhasorDomainSignal : IPhasorDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public PhasorDomainSignal(string unit)
		{
			Unit = unit;
			Interpreter = new PhasorDomainSignalInterpreter(this);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignal(IPhasorDomainSignal signal) : this(signal.Unit)
		{
			Copy(signal ?? throw new ArgumentNullException(nameof(signal)));
		}

		/// <summary>
		/// Constructor with parameter
		/// </summary>
		/// <param name="phasors"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public PhasorDomainSignal(IEnumerable<KeyValuePair<ISourceDescription, Complex>> phasors, string unit) : this(unit)
		{
			_Phasors = phasors?.ToDictionary((x) => x.Key, (x) => x.Value) ?? throw new ArgumentNullException(nameof(phasors));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Backing store for <see cref="Phasors"/>
		/// </summary>
		protected Dictionary<ISourceDescription, Complex> _Phasors { get; } = new Dictionary<ISourceDescription, Complex>();

		#endregion

		#region Public properties

		/// <summary>
		/// Unit to dislay
		/// </summary>
		public string Unit { get; }

		/// <summary>
		/// List with phasors adding to the signal
		/// </summary>
		public IDictionary<ISourceDescription, Complex> Phasors => _Phasors;

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
				if (_Phasors.Keys.Where((x) => x.Frequency == 0).FirstOrDefault() != null)
				{
					result |= SignalType.DC;
				}

				// Get the number of phasors
				var acPhasorsCount = Phasors.Keys.Where((x) => x.Frequency > 0).Count();

				// If it's greater than 0
				if (acPhasorsCount > 0)
				{
					// And greater than 1, set the multi AC flag
					if (acPhasorsCount > 1)
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

		#region Protected methods

		/// <summary>
		/// Adds <paramref name="value"/> to this <see cref="PhasorDomainSignal"/>. Either creates a new entry or adds the <paramref name="value"/>
		/// to the entry already existing for <paramref name="source"/>.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="value"></param>
		protected void AddPhasorHelper(ISourceDescription source, Complex value)
		{
			// If source is already present in the dictionary
			if(_Phasors.ContainsKey(source))
			{
				// Add the value to the stored value
				_Phasors[source] += value;
			}
			else
			{
				// Otherwise create a new entry
				_Phasors.Add(source, value);
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
		/// <exception cref="ArgumentNullException"></exception>
		public void Copy(IPhasorDomainSignal obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			// Clear the phasors and add obj's elements to it
			_Phasors.Clear();
			obj.Phasors.ForEach((x) => AddPhasorHelper(x.Key, x.Value));
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
		public PhasorDomainSignal CopyAndNegate() => new PhasorDomainSignal(Phasors.Select((phasor) =>
			new KeyValuePair<ISourceDescription, Complex>(phasor.Key, -phasor.Value)), Unit);

		/// <summary>
		/// Creates a copy of the signal in reversed direction (<see cref="DC"/> and each <see cref="Phasors"/> value is negated)
		/// </summary>
		/// <returns></returns>
		IPhasorDomainSignal IPhasorDomainSignal.CopyAndNegate() => CopyAndNegate();
		
		#endregion
	}
}