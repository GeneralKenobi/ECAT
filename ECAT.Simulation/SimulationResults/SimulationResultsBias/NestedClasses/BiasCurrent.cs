using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias : ISimulationResults
	{
		/// <summary>
		/// Provides functionality connected with storing, calculating and exposing currents and information about them in
		/// form of <see cref="IPhasorDomainSignal"/>s and <see cref="ISignalInformation"/>
		/// </summary>
		private class BiasCurrent : ICurrentDB, IBiasCurrent
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires two parameters, if either is null, an exception will be thrown
			/// </summary>
			/// <param name="voltageDrops">Object contain information about voltage drops calculated in simulation, can't be null</param>
			/// <param name="activeComponentCurrents">Currents produced by active components, can't be null</param>
			public BiasCurrent(IBiasVoltage voltageDrops, IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> activeComponentCurrents)
			{
				_VoltageDrops = voltageDrops ?? throw new ArgumentNullException(nameof(voltageDrops));

				_ActiveComponentsCurrentCache = new Dictionary<int, Tuple<IPhasorDomainSignal, SignalInformation, SignalInformation>>(
					activeComponentCurrents?.ToDictionary(
					// Key remains the same
					(x) => x.Key,
					// Value is constructor based on value in enumation's elements
					(x) => new Tuple<IPhasorDomainSignal, SignalInformation, SignalInformation>(
						// First tuple element is the signal
						x.Value,
						// Second is info constructed based on the signal
						new SignalInformation(x.Value),
						// Third is info constructed based on negation of the signal
						new SignalInformation(x.Value.CopyAndNegate())))
						// Finally if the activeComponentsCurrents was null, the ? operator included above will return null which will
						// be caught here and an exception will be thrown
						?? throw new ArgumentNullException(nameof(activeComponentCurrents)));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Contains information about voltage drops calculated in simulation
			/// </summary>
			private IBiasVoltage _VoltageDrops { get; }

			/// <summary>
			/// Cache with currents produced by <see cref="IVoltageSource"/>s, <see cref="IACVoltageSource"/>s and <see cref="IOpAmp"/>s
			/// Item1 is the data on which the signal is based, Item2 is information about the signal data, Item3 is information
			/// about negated signal data.
			/// </summary>
			private Dictionary<int, Tuple<IPhasorDomainSignal, SignalInformation, SignalInformation>> _ActiveComponentsCurrentCache { get; }

			/// <summary>
			/// Contains already computed currents, Item1 is in the standard direction, Item2 is in the reverse direction
			/// </summary>
			private Dictionary<IBaseComponent, Tuple<SignalInformation, SignalInformation>> _CurrentCache { get; } =
				new Dictionary<IBaseComponent, Tuple<SignalInformation, SignalInformation>>();

			#endregion
		}
	}
}