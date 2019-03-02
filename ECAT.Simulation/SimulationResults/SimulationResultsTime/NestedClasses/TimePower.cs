using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Linq;

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
			private bool TryConstructPower(ITwoTerminal twoTerminal, out ITimeDomainSignal power, bool voltageBA)
			{
				power = null;

				if (_VoltageDrops.TryGet(twoTerminal, out var voltage, voltageBA))
				{
					ITimeDomainSignal current = null;

					// Call different method depending on the type of the two terminal
					TypeSwitch.Construct().
						LazyCase<IResistor>((x) => _Currents.TryGet(x, out current)).
						LazyCase<ICapacitor>((x) => _Currents.TryGet(x, out current)).
						LazyCase<IActiveComponent>((x) => _Currents.TryGet(x.Index, out current)).
						Switch(twoTerminal);
					
					// If both voltage and current were obtained
					if(current != null)
					{
						var result = IoC.Resolve<ITimeDomainSignalMutable>(voltage.Samples, voltage.TimeStep, voltage.StartTime);

						// The result is a product of voltage and current waveforms
						// TODO: Think if power can be calculated as a sum of powers for each source, in any case null cannot be passed
						// as source description - replace it with some kind of default value.
						result.AddWaveform(null, voltage.FinalWaveform.MergeSelect(current.FinalWaveform, (x, y) => x * y));

						power = result;
					}
				}

				// If power is not null it means that it was successfully constructed
				return power != null;
			}

			#endregion

			#region Protected methods

			/// <summary>
			/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <returns></returns>
			protected override ITimeDomainSignal CopyAndNegate(ITimeDomainSignal signal) => signal.CopyAndNegate();

			/// <summary>
			/// Tries to construct power for <paramref name="component"/>, on success assigns it to <paramref name="power"/> and
			/// true, on failure assigns null to <paramref name="power"/> and returns false.
			/// </summary>
			/// <param name="component"></param>
			/// <param name="power"></param>
			/// <returns></returns>
			protected override bool TryConstructPower(IBaseComponent component, out ITimeDomainSignal power, bool voltageBA)
			{
				ITimeDomainSignal constructedPower = null;

				TypeSwitch.Construct().
					LazyCase<ITwoTerminal>((x) => TryConstructPower(x, out constructedPower, voltageBA)).
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
			public ISignalInformation Get(IResistor resistor, bool voltageBA) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(resistor, voltageBA) &&
				_Cache.TryGetValue(Tuple.Create<IBaseComponent, bool>(resistor, voltageBA), out var power) ? power.Item2 : null;

			/// <summary>
			/// Gets information about power dissipated on an <see cref="ICapacitor"/>
			/// </summary>
			/// <param name="capacitor"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICapacitor capacitor, bool voltageBA) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(capacitor, voltageBA) &&
				_Cache.TryGetValue(Tuple.Create<IBaseComponent, bool>(capacitor, voltageBA), out var power) ? power.Item2 : null;


			/// <summary>
			/// Gets information about power on an <see cref="ICurrentSource"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICurrentSource currentSource, bool voltageBA) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(currentSource, voltageBA) &&
				_Cache.TryGetValue(Tuple.Create<IBaseComponent, bool>(currentSource, voltageBA), out var power) ? power.Item2 : null;

			/// <summary>
			/// Gets information about power on an <see cref="IDCVoltageSource"/>
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IDCVoltageSource voltageSource, bool voltageBA) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(voltageSource, voltageBA) &&
				_Cache.TryGetValue(Tuple.Create<IBaseComponent, bool>(voltageSource, voltageBA), out var power) ? power.Item2 : null;

			/// <summary>
			/// Gets information about power on an <see cref="IACVoltageSource"/>. If the <paramref name="current"/> is composed of
			/// phasors with different frequency than that of <paramref name="voltageSource"/> the average power will be assigned
			/// <see cref="Double.NaN"/> (it's impossible to calculate it using only phasors). Doesn't compute maximum/minimum
			/// instantenous power.
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IACVoltageSource voltageSource, bool voltageBA) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(voltageSource, voltageBA) &&
				_Cache.TryGetValue(Tuple.Create<IBaseComponent, bool>(voltageSource, voltageBA), out var power) ? power.Item2 : null;

			#endregion
		}
	}
}