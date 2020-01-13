using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	partial class SimulationResultsTime
	{
		/// <summary>
		/// Provides functionality connected with storing, calculating and exposing currents and information about them in
		/// form of <see cref="ITimeDomainSignal"/>s and <see cref="ISignalInformation"/>
		/// </summary>
		private class TimeCurrent : CurrentCache<ITimeDomainSignal>, ICurrentDB, ICurrentSignalDB<ITimeDomainSignal>
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires two parameters, if either is null, an exception will be thrown
			/// </summary>
			/// <param name="voltageDrops">Object contain information about voltage drops calculated in simulation, can't be null</param>
			/// <param name="activeComponentsCurrents">Currents produced by active components, can't be null</param>
			public TimeCurrent(IVoltageSignalDB<ITimeDomainSignal> voltageDrops,
				IEnumerable<KeyValuePair<int, ITimeDomainSignal>> activeComponentsCurrents) : base(activeComponentsCurrents)
			{
				_VoltageDrops = voltageDrops ?? throw new ArgumentNullException(nameof(voltageDrops));		
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Contains information about voltage drops calculated in simulation
			/// </summary>
			private IVoltageSignalDB<ITimeDomainSignal> _VoltageDrops { get; }

			#endregion

			#region Private methods

			/// <summary>
			/// Attempts to obtain a current for some <see cref="ITwoTerminal"/> <paramref name="component"/>
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <param name="current"></param>
			/// <returns></returns>
			private bool TryGetStandardTwoTerminalCurrent(ITwoTerminal component, bool voltageBA, out ITimeDomainSignal current)
			{
				// Check if the current may be obtained from cache
				if(TryEnableCurrent(component, voltageBA) &&
					// If so, try to get it from the cache (this condition should always be true if the previous one is true)
					_Cache.TryGetValue(new Tuple<ITwoTerminal, bool>(component, voltageBA), out var currentPackage))
				{
					current = currentPackage.Item1;
					return true;
				}
				else
				{
					current = null;
					return false;
				}
			}

			/// <summary>
			/// Attempts to obtain a current for some <see cref="ITwoTerminal"/> <paramref name="component"/>, if not successful
			/// returns null
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>			
			/// <returns></returns>
			private ISignalInformation GetStandardTwoTerminalCurrent(ITwoTerminal component, bool voltageBA)
			{
				// Check if the current may be obtained from cache
				if (TryEnableCurrent(component, voltageBA) &&
					// If so, try to get it from the cache (this condition should always be true if the previous one is true)
					_Cache.TryGetValue(new Tuple<ITwoTerminal, bool>(component, voltageBA), out var currentPackage))
				{
					return currentPackage.Item2;
				}
				else
				{
					return null;
				}
			}

			#endregion

			#region Protected methods

			/// <summary>
			/// Tries to construct a current for <paramref name="element"/>, returns true on success
			/// </summary>
			/// <param name="element"></param>
			/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
			/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
			/// that direction was reversed</param>
			/// <param name="current">Current constructed if successful, null otherwise</param>
			/// <returns></returns>
			protected override bool TryConstructCurrent(ITwoTerminal element, bool voltageBA, out ITimeDomainSignal current)
			{
				// Try to get voltage drop across the element
				if (_VoltageDrops.TryGet(element, out var voltageDrop, voltageBA))
				{
					// If successful, create a new current signal based on it, cache it
					var result = IoC.Resolve<ITimeDomainSignalMutable>(voltageDrop.Samples, voltageDrop.Step, voltageDrop.StartSample, IoC.Resolve<ISIUnits>().CurrentShort);

					// Get the minimum frequency - it is needed for capacitor waveform shifting, check if there are any waveforms, if not
					// just assign 0 (technically no waveforms result in a zero wave, which is DC)
					var acWaveforms = voltageDrop.ComposingWaveforms.Where((x) => x.Key.Frequency > 0);
					var minACFrequency = acWaveforms.Count() > 0 ? acWaveforms.Min((x) => x.Key.Frequency) : 0;

					// Current is composed of each voltage waveform times admittance of the element
					foreach (var waveform in voltageDrop.ComposingWaveforms)
					{
						// Get magnitude of element's admittance
						var admittanceMagnitude = element.GetAdmittance(waveform.Key.Frequency).Magnitude;

						// Current waveform is the product of voltage waveforrm and magnitude
						var finalWaveform = waveform.Value.Select((x) => x * admittanceMagnitude);

						// Introduce phase shift for capacitors and inductors - but only if minimum AC frequency is greater than 0, if it's not then there were
						// no AC voltage sources and so no current will flow through any capacitor
						if (minACFrequency > 0 && waveform.Key.Frequency > 0)
						{
							if(element is ICapacitor)
							{
								// Each wave has to be shifted by pi / 2 but only in its period.
								// For example, a wave with frequency 2 times the lowest frequency has to be shifted by total of pi / 4 - because
								// there are 2 periods of it in the full waveform. This relation is given by minimum frequency / wave frequency
								finalWaveform = WaveformBuilder.ShiftWaveform(finalWaveform, minACFrequency / waveform.Key.Frequency * Math.PI / 2 );
							}
							else if (element is IInductor)
							{
								// Analogous for inductors
								finalWaveform = WaveformBuilder.ShiftWaveform(finalWaveform, 2 * Math.PI - minACFrequency / waveform.Key.Frequency * Math.PI / 2);
							}
						}

						// Add the waveform to the final waveform
						result.AddWaveform(waveform.Key, finalWaveform);
					}
					
					current = result;

					// And return success
					return true;
				}
				else
				{
					// Return failure					
					current = null;
					return false;
				}
			}

			/// <summary>
			/// Copies and negates <paramref name="signal"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <returns></returns>
			protected override ITimeDomainSignal CopyAndNegate(ITimeDomainSignal signal) => signal.CopyAndNegate();

			#endregion

			#region Public methods

			#region ICurrentBias Interface

			/// <summary>
			/// Gets current flowing through an <see cref="IResistor"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="resistor"></param>
			/// <param name="current"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public bool TryGet(IResistor resistor, out ITimeDomainSignal current, bool voltageBA = true) =>
				TryGetStandardTwoTerminalCurrent(resistor, voltageBA, out current);
			
			/// <summary>
			/// Gets current flowing through an <see cref="ICapacitor"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="capacitor"></param>
			/// <param name="current"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public bool TryGet(ICapacitor capacitor, out ITimeDomainSignal current, bool voltageBA = true) =>
				TryGetStandardTwoTerminalCurrent(capacitor, voltageBA, out current);

			/// <summary>
			/// Gets current flowing through an <see cref="IInductor"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="inductor"></param>
			/// <paramref name="current"></paramref>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public bool TryGet(IInductor inductor, out ITimeDomainSignal current, bool voltageBA = true) =>
				TryGetStandardTwoTerminalCurrent(inductor, voltageBA, out current);

			/// <summary>
			/// Gets current produced by some <see cref="IActiveComponent"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <paramref name="current"></paramref>
			/// <param name="reverseDirection">If true, the direction of the current will be reversed (with the respect to
			/// the normal direction obtained in simulation)</param>
			/// <returns></returns>
			public bool TryGet(int activeComponentIndex, out ITimeDomainSignal current, bool reverseDirection = false) =>
				TryGetActiveComponentCurrent(activeComponentIndex, out current, reverseDirection);

			#endregion

			#region ICurrentDB

			/// <summary>
			/// Gets information about current flowing through an <see cref="IResistor"/> or null if unsuccessful
			/// </summary>		
			/// <param name="resistor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation Get(IResistor resistor, bool voltageBA) =>
				GetStandardTwoTerminalCurrent(resistor, voltageBA);

			/// <summary>
			/// Gets information about current flowing through an <see cref="ICapacitor"/> or null if unsuccessful
			/// </summary>		
			/// <param name="capacitor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation Get(ICapacitor capacitor, bool voltageBA) =>
				GetStandardTwoTerminalCurrent(capacitor, voltageBA);

			/// <summary>
			/// Gets information about current flowing through an <see cref="IInductor"/> or null if unsuccessful
			/// </summary>		
			/// <param name="inductor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation Get(IInductor inductor, bool voltageBA) =>
				GetStandardTwoTerminalCurrent(inductor, voltageBA);

			/// <summary>
			/// Returns current produced by some <see cref="IActiveComponent"/>. If simulation was not yet performed or the current can't be
			/// found returns null
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
			/// by convention for the specific element (obtained during simulation)</param>
			/// <returns></returns>
			public ISignalInformation Get(int activeComponentIndex, bool reverseDirection) =>
				TryGetActiveComponentCurrent(activeComponentIndex, out ISignalInformation info, reverseDirection) ? info : null;

			#endregion

			#endregion
		}
	}
}