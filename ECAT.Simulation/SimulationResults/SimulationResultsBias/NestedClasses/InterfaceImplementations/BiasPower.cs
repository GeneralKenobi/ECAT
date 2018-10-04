using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias : ISimulationResults
	{
		/// <summary>
		/// Provides functionality connected with storing, calculating and exposing power information in
		/// form of <see cref="ISignalInformation"/>
		/// </summary>
		private class BiasPower : IPowerDB
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires voltage drops and currents
			/// </summary>
			/// <param name="voltageDrops"></param>
			/// <param name="currents"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public BiasPower(IBiasVoltage voltageDrops, IBiasCurrent currents)
			{
				_VoltageDrops = voltageDrops ?? throw new ArgumentNullException(nameof(voltageDrops));
				_Currents = currents ?? throw new ArgumentNullException(nameof(currents));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Contains information about voltage drops calculated in simulation
			/// </summary>
			private IBiasVoltage _VoltageDrops { get; }

			/// <summary>
			/// Contains information about currents calculated in simulation
			/// </summary>
			private IBiasCurrent _Currents { get; }

			/// <summary>
			/// Contains already computed <see cref="ISignalInformation"/>s
			/// </summary>
			private Dictionary<IBaseComponent, ISignalInformation> _Cache { get; } = new Dictionary<IBaseComponent,ISignalInformation>();

			#endregion

			#region Private methods

			/// <summary>
			/// Caches the <paramref name="power"/> in <see cref="_Cache"/>
			/// </summary>
			/// <param name="component">Component for which the current flow is considered</param>
			/// <param name="power"></param>
			private void CachePower(IBaseComponent component, ISignalData power) =>
				_Cache.Add(
					component,
					IoC.Resolve<ISignalInformationFactory>().Construct(power, IoC.Resolve<ICommonSignalDescriptions>().Power));

			/// <summary>
			/// Returns true if power for <paramref name="resistor"/> can be obtained from <see cref="_Cache"/>
			/// </summary>
			/// <param name="resistor"></param>
			/// <returns></returns>
			private bool TryEnablePower(IResistor resistor) =>
				// Check if the cache already contains an entry, otherwise try to construct it
				_Cache.ContainsKey(resistor) || TryConstructPower(resistor);

			/// <summary>
			/// Tries to construct a power for <paramref name="resistor"/>, returns true on success
			/// </summary>
			/// <param name="element"></param>
			/// <returns></returns>
			private bool TryConstructPower(IResistor resistor)
			{
				// Try to get voltage drop across the element
				if (_VoltageDrops.TryGet(resistor, out var voltageDrop))
				{
					// If successful, create a new power signal based on it, cache it
					CachePower(resistor, 
						IoC.Resolve<IPhasorDomainSignal>(
						Math.Pow(voltageDrop.DC, 2) / resistor.Resistance,
						voltageDrop.Phasors.
						Select((v) => new KeyValuePair<double, Complex>(v.Key, Complex.Pow(v.Value, 2) / resistor.Resistance))));

					// And return success
					return true;
				}
				else
				{
					// Return failure					
					return false;
				}
			}

			/// <summary>
			/// Returns true if power for <paramref name="voltageSource"/> can be obtained from <see cref="_Cache"/>
			/// </summary>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			private bool TryEnablePower(IVoltageSource voltageSource) =>
				// Check if the cache already contains an entry, otherwise try to construct it
				_Cache.ContainsKey(voltageSource) || TryConstructPower(voltageSource);

			/// <summary>
			/// Tries to construct a power for <paramref name="voltageSource"/>, returns true on success
			/// </summary>
			/// <param name="element"></param>
			/// <returns></returns>
			private bool TryConstructPower(IVoltageSource voltageSource)
			{
				// Try to get voltage drop across the element
				if (_Currents.TryGet(voltageSource.ActiveComponentIndex, out var current))
				{
					// If successful, create a new power signal based on it, cache it
					CachePower(voltageSource, new CharacteristicValuesPowerSignal(
						current.Interpreter.Maximum() * voltageSource.ProducedDCVoltage,
						current.Interpreter.Minimum() * voltageSource.ProducedDCVoltage,
						-current.DC * voltageSource.ProducedDCVoltage));
						

					// And return success
					return true;
				}
				else
				{
					// Return failure					
					return false;
				}
			}

			/// <summary>
			/// Returns true if power for <paramref name="voltageSource"/> can be obtained from <see cref="_Cache"/>
			/// </summary>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			private bool TryEnablePower(IACVoltageSource voltageSource) =>
				// Check if the cache already contains an entry, otherwise try to construct it
				_Cache.ContainsKey(voltageSource) || TryConstructPower(voltageSource);

			/// <summary>
			/// Tries to construct a power for <paramref name="voltageSource"/>, returns true on success
			/// </summary>
			/// <param name="element"></param>
			/// <returns></returns>
			private bool TryConstructPower(IACVoltageSource voltageSource)
			{
				// Try to get voltage drop across the element
				if (_Currents.TryGet(voltageSource.ActiveComponentIndex, out var current))
				{
					// If successful, create a new power signal based on it, cache it
					CachePower(voltageSource, new CharacteristicValuesPowerSignal(
						// Min and max can't be calculated
						double.NaN,
						double.NaN,
						// If there is only one phasor then average can be calculated, otherwise not
						current.Phasors.Count() != 1 ? double.NaN :
						current.Interpreter.RMS() * Math.Sqrt(Math.Pow(voltageSource.ProducedDCVoltage, 2) +
						Math.Pow(voltageSource.PeakProducedVoltage, 2) / 2) * Math.Cos(current.Phasors.First().Value.Phase)));
					
					// And return success
					return true;
				}
				else
				{
					// Return failure					
					return false;
				}
			}

			/// <summary>
			/// Returns true if power for <paramref name="currentSource"/> can be obtained from <see cref="_Cache"/>
			/// </summary>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			private bool TryEnablePower(ICurrentSource currentSource) =>
				// Check if the cache already contains an entry, otherwise try to construct it
				_Cache.ContainsKey(currentSource) || TryConstructPower(currentSource);

			/// <summary>
			/// Tries to construct a power for <paramref name="currentSource"/>, returns true on success
			/// </summary>
			/// <param name="element"></param>
			/// <returns></returns>
			private bool TryConstructPower(ICurrentSource currentSource)
			{
				// Try to get voltage drop across the element
				if (_VoltageDrops.TryGet(currentSource, out var voltageDrop))
				{
					// If successful, create a new power signal based on it, cache it
					CachePower(currentSource, new CharacteristicValuesPowerSignal(
						// Minimum power (the maximum supplied or the least dissipated, depending on actual values)
						// It's the minimum voltage drop minus twice DC voltage drop times current. (Minimum already has +VDC in it so in order
						// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
						(voltageDrop.Interpreter.Maximum() - 2 * voltageDrop.DC) * currentSource.ProducedCurrent,
						// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
						// It's the maximum voltage drop minus twice DC voltage drop times current. (Maximum already has +VDC in it so in order
						// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
						(voltageDrop.Interpreter.Maximum() - 2 * voltageDrop.DC) * currentSource.ProducedCurrent,
						// Average is negative voltage drop times produced current (to abide passive sign convention)						
						-voltageDrop.DC * currentSource.ProducedCurrent));

					// And return success
					return true;
				}
				else
				{
					// Return failure					
					return false;
				}
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
				TryEnablePower(resistor) && _Cache.TryGetValue(resistor, out var power) ? power : null;
			

			/// <summary>
			/// Gets information about power on an <see cref="ICurrentSource"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(ICurrentSource currentSource) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(currentSource) && _Cache.TryGetValue(currentSource, out var power) ? power : null;

			/// <summary>
			/// Gets information about power on an <see cref="IVoltageSource"/>
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public ISignalInformation Get(IVoltageSource voltageSource) =>
				// Check if power can be enabled and if it can be fetched, if so return it, otherwise return null
				TryEnablePower(voltageSource) && _Cache.TryGetValue(voltageSource, out var power) ? power : null;

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
				TryEnablePower(voltageSource) && _Cache.TryGetValue(voltageSource, out var power) ? power : null;

			#endregion
		}
	}
}