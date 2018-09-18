using CSharpEnhanced.CoreClasses;
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
		/// Provides functionality connected with storing, calculating and exposing voltage drops and information about them in
		/// form of <see cref="IPhasorDomainSignal"/>s and <see cref="ISignalInformation"/>
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
				_Cache.Add(component, IoC.Resolve<ISignalInformationFactory>().Construct(power));

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
				if (_VoltageDrops.TryGetVoltageDrop(resistor, out var voltageDrop))
				{
					// If successful, create a new power signal based on it, cache it
					CachePower(resistor, 
						IoC.Resolve<IPhasorDomainSignalFactory>().Construct(
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
				if (_Currents.TryGetCurrent(voltageSource.ActiveComponentIndex, out var current))
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
				if (_Currents.TryGetCurrent(voltageSource.ActiveComponentIndex, out var current))
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
				if (_VoltageDrops.TryGetVoltageDrop(currentSource, out var voltageDrop))
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
			public IPowerInformation GetPower(IResistor resistor)
			{
				// Check if there already is a cached entry
				if (_Cache.TryGetValue(resistor, out var power))
				{
					return power;
				}

				// Get the voltage drop across the resistor
				var voltageDrop = ResolveVoltageDrop(resistor);

				// If not create a new power info
				var result = new PowerInformation()
				{
					// Average power on a resistor is a sqaure of DC voltage plus half of squares of AC magnitudes (RMS values) times
					// the conductance of the resistor
					Average = (voltageDrop.Phasors.Sum((phasor) => Math.Pow(phasor.Value.Magnitude, 2)) / 2 +
					Math.Pow(voltageDrop.DC, 2)) * resistor.GetConductance(),

					// Maximum occurs for maximum voltage drop and is simply a square of voltage times conductance
					Maximum = Math.Pow(voltageDrop.Interpreter.Maximum(), 2) * resistor.GetConductance(),
				};

				// Cache it
				CachePower(resistor, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="ICurrentSource"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(ICurrentSource currentSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(currentSource, out var power))
				{
					return power;
				}

				var voltageDrop = ResolveVoltageDrop(currentSource);

				// Average is negative voltage drop times produced current (to abide passive sign convention)
				var result = new PowerInformation()
				{
					Average = -voltageDrop.DC * currentSource.ProducedCurrent,
				};

				// Minimum power (the maximum supplied or the least dissipated, depending on actual values)
				// It's the minimum voltage drop minus twice DC voltage drop times current. (Minimum already has +VDC in it so in order
				// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
				result.Minimum = (voltageDrop.Interpreter.Maximum() - 2 * voltageDrop.DC) * currentSource.ProducedCurrent;

				// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
				// It's the maximum voltage drop minus twice DC voltage drop times current. (Maximum already has +VDC in it so in order
				// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
				result.Maximum = (voltageDrop.Interpreter.Maximum() - 2 * voltageDrop.DC) * currentSource.ProducedCurrent;

				// Cache the calculated value
				CachePower(currentSource, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="IVoltageSource"/>
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(IVoltageSource voltageSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(voltageSource, out var power))
				{
					return power;
				}

				if (!_ActiveComponentsCurrentCache.TryGetValue(voltageSource.ActiveComponentIndex, out var currentPackage))
				{
					return new PowerInformation();
				}

				var current = currentPackage.Item1;

				// Average is voltage drop times produced current (current is assumed to flow right to left in standard convention,
				// current produced flows left to right so produced power is negative)
				var result = new PowerInformation()
				{
					Average = -current.DC * voltageSource.ProducedDCVoltage,
				};

				// Minimum power (the maximum supplied or the least dissipated, depending on actual values)				
				result.Minimum = current.Interpreter.Minimum() * voltageSource.ProducedDCVoltage;

				// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
				result.Maximum = current.Interpreter.Maximum() * voltageSource.ProducedDCVoltage;

				// Cache the calculated value
				CachePower(voltageSource, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="IACVoltageSource"/>. If the <paramref name="current"/> is composed of
			/// phasors with different frequency than that of <paramref name="voltageSource"/> the average power will be assigned
			/// <see cref="Double.NaN"/> (it's impossible to calculate it using only phasors). Doesn't compute maximum/minimum
			/// instantenous power.
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(IACVoltageSource voltageSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(voltageSource, out var power))
				{
					return power;
				}

				if (!_ActiveComponentsCurrentCache.TryGetValue(voltageSource.ActiveComponentIndex, out var currentPackage))
				{
					return new PowerInformation();
				}

				var current = currentPackage.Item1;

				// Create a new info, initialize Minimum and Maximum with NaN to indicate they couldn't have been calculated
				var result = new PowerInformation()
				{
					Minimum = double.NaN,
					Maximum = double.NaN,
				};

				// If it has only ony AC
				if (current.Type.HasFlag(SignalType.AC))
				{
					// If there is more than one phasor or the phasor, for some reason, has a different frequency than the source
					if (current.Type.HasFlag(SignalType.MultipleAC) || current.Phasors.First().Key != voltageSource.Frequency)
					{
						// Assign NaN as the average value cannot be easily computed
						result.Average = double.NaN;
					}
					else
					{
						// Get the only phasor composing the 
						var singlePhasors = current.Phasors.First();

						// Calculate the average as Vrms*Irms*cos(phiV - phiI)
						// TODO: When IAsyncVoltageSource has phase shift, include it in the formula
						result.Average = current.Interpreter.RMS() * Math.Sqrt(Math.Pow(voltageSource.ProducedDCVoltage, 2) +
							Math.Pow(voltageSource.PeakProducedVoltage, 2) / 2) * Math.Cos(singlePhasors.Value.Phase);
					}
				}

				// Cache the calculated value
				CachePower(voltageSource, result);

				// And return it
				return result;
			}

			#endregion
		}
	}
}