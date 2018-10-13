using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	partial class SimulationResultsTime
	{
		/// <summary>
		/// Provides functionality connected with storing, calculating and exposing power information in
		/// form of <see cref="ISignalInformation"/>
		/// </summary>
		private class TimePower : PowerCache<ITimeDomainSignal>, IPowerDB
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires voltage drops and currents
			/// </summary>
			/// <param name="voltageDrops"></param>
			/// <param name="currents"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public TimePower(IVoltageSignalDB<ITimeDomainSignal> voltageDrops, ICurrentSignalDB<ITimeDomainSignal> currents)
			{
				_VoltageDrops = voltageDrops ?? throw new ArgumentNullException(nameof(voltageDrops));
				_Currents = currents ?? throw new ArgumentNullException(nameof(currents));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Contains information about voltage drops calculated in simulation
			/// </summary>
			private IVoltageSignalDB<ITimeDomainSignal> _VoltageDrops { get; }

			/// <summary>
			/// Contains information about currents calculated in simulation
			/// </summary>
			private ICurrentSignalDB<ITimeDomainSignal> _Currents { get; }

			#endregion

			#region Private methods

			/// <summary>
			/// Tries to constract a power for an <see cref="ITwoTerminal"/>
			/// </summary>
			/// <param name="twoTerminal"></param>
			/// <param name="power"></param>
			/// <returns></returns>
			private bool TryConstructPower(ITwoTerminal twoTerminal, out ITimeDomainSignal power)
			{
				power = null;

				if (_VoltageDrops.TryGet(twoTerminal, out var voltage))
				{
					ITimeDomainSignal current = null;

					// Call different method depending on the type of the two terminal
					TypeSwitch.Construct().
						LazyCase<IResistor>((x) => _Currents.TryGet(x, out current)).
						LazyCase<ICapacitor>((x) => _Currents.TryGet(x, out current)).
						LazyCase<IActiveComponent>((x) => _Currents.TryGet(x.ActiveComponentIndex, out current)).
						Switch(twoTerminal);
					
					// If both voltage and current were obtained
					if(current != null)
					{
						power = IoC.Resolve<ITimeDomainSignal>(
							voltage.InstantenousValues.MergeSelect(current.InstantenousValues, (x, y) => x * y),
							voltage.TimeStep,
							current.StartTime);
					}
				}

				// If power is not null it means that it was successfully constructed
				return power != null;
			}

			#endregion

			#region Protected methods

			/// <summary>
			/// Tries to construct power for <paramref name="component"/>, on success assigns it to <paramref name="power"/> and
			/// true, on failure assigns null to <paramref name="power"/> and returns false.
			/// </summary>
			/// <param name="component"></param>
			/// <param name="power"></param>
			/// <returns></returns>
			protected override bool TryConstructPower(IBaseComponent component, out ITimeDomainSignal power)
			{
				ITimeDomainSignal constructedPower = null;

				TypeSwitch.Construct().
					LazyCase<ITwoTerminal>((x) => TryConstructPower(x, out constructedPower)).
					Switch(component);

				power = constructedPower;
				return power != null;
			}

			#endregion

			#region Public methods

			/// <summary>
			/// Gets information about power dissipated on an <see cref="IResistor"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="resistor"></param>
			/// <returns></returns>
			public ISignalInformation Get(IResistor resistor) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(resistor) && _Cache.TryGetValue(resistor, out var power) ? power.Item2 : null;
			

			/// <summary>
			/// Gets information about power on an <see cref="ICurrentSource"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICurrentSource currentSource) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(currentSource) && _Cache.TryGetValue(currentSource, out var power) ? power.Item2 : null;

			/// <summary>
			/// Gets information about power on an <see cref="IVoltageSource"/>
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IVoltageSource voltageSource) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(voltageSource) && _Cache.TryGetValue(voltageSource, out var power) ? power.Item2 : null;

			/// <summary>
			/// Gets information about power on an <see cref="IACVoltageSource"/>. If the <paramref name="current"/> is composed of
			/// phasors with different frequency than that of <paramref name="voltageSource"/> the average power will be assigned
			/// <see cref="Double.NaN"/> (it's impossible to calculate it using only phasors). Doesn't compute maximum/minimum
			/// instantenous power.
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IACVoltageSource voltageSource) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(voltageSource) && _Cache.TryGetValue(voltageSource, out var power) ? power.Item2 : null;

			#endregion
		}
	}
}