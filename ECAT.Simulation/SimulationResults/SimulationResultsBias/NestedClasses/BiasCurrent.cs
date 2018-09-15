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

				// Create a new dictionary
				_ActiveComponentsCache = new Dictionary<Tuple<int, bool>, Tuple<IPhasorDomainSignal, ISignalInformation>>(
					// Check if parameter is not null
					activeComponentCurrents?.
					// Project it to a tuple with the int key and bool (false, indicating no direction change) and the value
					Select((x) => new KeyValuePair<Tuple<int, bool>, IPhasorDomainSignal>(new Tuple<int, bool>(x.Key, false), x.Value)).
					// Concat it with the same currents, this time
					Concat(activeComponentCurrents.
					// Project them to a tuple with the int key and bool (true, indicating direction was reversed) and the value negated
					Select((x) => new KeyValuePair<Tuple<int, bool>, IPhasorDomainSignal>(new Tuple<int, bool>(x.Key, true), x.Value.CopyAndNegate()))).
					// Finally transform it to a dictionary of required types
					ToDictionary(
					// Key stays the same
					(x) => x.Key,
					// Value is the signal and information based on it
					(x) => new Tuple<IPhasorDomainSignal, ISignalInformation>(x.Value, new SignalInformation(x.Value)))
					// If the null check above caught a null value, this operator will result in the exception being thrown
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
			/// For key Item1 is the index of the <see cref="IActiveComponent"/>, Item2 determines whether direction was reversed.
			/// For value Item1 is the data on which the signal is based, Item2 is information about the signal data.
			/// </summary>
			private Dictionary<Tuple<int, bool>, Tuple<IPhasorDomainSignal, ISignalInformation>> _ActiveComponentsCache { get; }

			/// <summary>
			/// Contains already computed currents. Key is component through which the current flows and a bool which indicates direction
			/// of voltage drop used to calculate the current: true for voltage from <see cref="ITwoTerminal.TerminalA"/> (reference
			/// node) to <see cref="ITwoTerminal.TerminalB"/>, false for reverse direction. Value is the current (Item1) and
			/// information build based on it (Item2)
			/// </summary>
			private Dictionary<Tuple<ITwoTerminal, bool>, Tuple<IPhasorDomainSignal, ISignalInformation>> _Cache { get; } =
				new Dictionary<Tuple<ITwoTerminal, bool>, Tuple<IPhasorDomainSignal, ISignalInformation>>(
					new CustomEqualityComparer<Tuple<ITwoTerminal, bool>>(
					// Compare the elements of the Tuples, not tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			#endregion

			#region Private methods

			/// <summary>
			/// If <see cref="_Cache"/> does not contain an entry with key given by <paramref name="component"/> and
			/// <paramref name="voltageBA"/>, caches <paramref name="signal"/>, otherwise doesn't do anything
			/// </summary>
			/// <param name="signal"></param>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
			/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
			/// that direction was reversed</param>
			private void CacheHelper(IPhasorDomainSignal signal, ITwoTerminal component, bool voltageBA)
			{
				if (!_Cache.ContainsKey(new Tuple<ITwoTerminal, bool>(component, voltageBA)))
				{
					_Cache.Add(new Tuple<ITwoTerminal, bool>(component, voltageBA),
						new Tuple<IPhasorDomainSignal, ISignalInformation>(signal, new SignalInformation(signal)));
				}
			}

			/// <summary>
			/// Caches the <paramref name="signal"/> as well as its negated copy (with inverted indexes) into <see cref="_Cache"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
			/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
			/// that direction was reversed</param>
			private void CacheCurrent(IPhasorDomainSignal signal, ITwoTerminal component, bool voltageBA)
			{
				// Cache the original
				CacheHelper(signal, component, voltageBA);

				// And cache the reversed one
				CacheHelper(signal.CopyAndNegate(), component, !voltageBA);
			}

			/// <summary>
			/// Returns a DC current flowing through a two terminal
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="twoTerminal"></param>
			/// <returns></returns>
			private double GetPassiveTwoTerminalDCCurrent(IPhasorDomainSignal voltageDrop, ITwoTerminal twoTerminal) =>
				voltageDrop.DC * twoTerminal.GetConductance();

			/// <summary>
			/// Returns AC current phasors for a two terminal
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="twoTerminal"></param>
			/// <returns></returns>
			private IEnumerable<KeyValuePair<double, Complex>> GetPassiveTwoTerminalACCurrentPhasors(
				IPhasorDomainSignal voltageDrop, ITwoTerminal twoTerminal) =>
				voltageDrop.ComposingPhasors.Select((phasor) =>
				new KeyValuePair<double, Complex>(phasor.Key, phasor.Value * twoTerminal.GetAdmittance(phasor.Key)));

			/// <summary>
			/// Tries to construct a current for <paramref name="element"/>, returns true on success
			/// </summary>
			/// <param name="element"></param>
			/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
			/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
			/// that direction was reversed</param>
			/// <returns></returns>
			private bool TryConstructCurrent(ITwoTerminal element, bool voltageBA)
			{
				// Try to get voltage drop across the element
				if (_VoltageDrops.TryGetVoltageDrop(element, out var voltageDrop, voltageBA))
				{
					// If successful, create a new current signal based on it
					var current = new PhasorDomainSignal()
					{
						DC = GetPassiveTwoTerminalDCCurrent(voltageDrop, element),
						ComposingPhasors = GetPassiveTwoTerminalACCurrentPhasors(voltageDrop, element),
					};

					// Cache it
					CacheCurrent(current, element, voltageBA);

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
			/// Checks if current through <paramref name="element"/> can be obtained from cache (<see cref="_Cache"/>), if not performs
			/// all possible actions to create it and cache it. Returns true if, at the end of the method call, the current may be
			/// obtained from <see cref="_Cache"/>, false otherwise.
			/// </summary>
			/// <param name="element">Element for which the current is considered</param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			private bool TryEnableCurrent(ITwoTerminal element, bool voltageBA) =>
				// If there was a cached entry already return it
				_Cache.ContainsKey(new Tuple<ITwoTerminal, bool>(element, voltageBA)) || TryConstructCurrent(element, voltageBA);

			/// <summary>
			/// Attempts to obtain a current for some <see cref="ITwoTerminal"/> <paramref name="component"/>
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <param name="current"></param>
			/// <returns></returns>
			private bool TryGetStandardTwoTerminalCurrent(ITwoTerminal component, bool voltageBA, out IPhasorDomainSignal current)
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
			public bool TryGetCurrent(IResistor resistor, out IPhasorDomainSignal current, bool voltageBA = true) =>
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
			public bool GetCurrent(ICapacitor capacitor, out IPhasorDomainSignal current, bool voltageBA = true) =>
				TryGetStandardTwoTerminalCurrent(capacitor, voltageBA, out current);

			/// <summary>
			/// Gets current produced by some <see cref="IActiveComponent"/> or null if unsuccessful and stores it in
			/// <paramref name="current"/>. Returns true on success, false otherwise.
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <paramref name="current"></paramref>
			/// <param name="reverseDirection">If true, the direction of the current will be reversed (with the respect to
			/// the normal direction obtained in simulation)</param>
			/// <returns></returns>
			public bool GetCurrent(int activeComponentIndex, out IPhasorDomainSignal current, bool reverseDirection = false)
			{
				// Check if the current is in the cache (If it was not provided during object construction, it's not possible to
				// calculate it now)
				if(_ActiveComponentsCache.TryGetValue(
					new Tuple<int, bool>(activeComponentIndex, reverseDirection) , out var currentPackage))
				{
					// If so, assign it and return success
					current = currentPackage.Item1;
					return true;
				}
				else
				{
					// Otherwise assign null and return false
					current = null;
					return false;
				}
			}

			#endregion

			#region ICurrentDB

			/// <summary>
			/// Gets information about current flowing through an <see cref="IResistor"/> or null if unsuccessful
			/// </summary>		
			/// <param name="resistor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation GetCurrent(IResistor resistor, bool voltageBA) =>
				GetStandardTwoTerminalCurrent(resistor, voltageBA);

			/// <summary>
			/// Gets information about current flowing through an <see cref="ICapacitor"/> or null if unsuccessful
			/// </summary>		
			/// <param name="capacitor"></param>
			/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
			/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
			/// <returns></returns>
			public ISignalInformation GetCurrent(ICapacitor capacitor, bool voltageBA) =>
				GetStandardTwoTerminalCurrent(capacitor, voltageBA);

			/// <summary>
			/// Returns current produced by some <see cref="IActiveComponent"/>. If simulation was not yet performed or the current can't be
			/// found returns null
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
			/// by convention for the specific element (obtained during simulation)</param>
			/// <returns></returns>
			public ISignalInformation GetCurrent(int activeComponentIndex, bool reverseDirection)
			{				
				// Check if the current is in the cache (If it was not provided during object construction, it's not possible to
				// calculate it now)
				if (_ActiveComponentsCache.TryGetValue(
					new Tuple<int, bool>(activeComponentIndex, reverseDirection), out var currentPackage))
				{
					// If so, return it
					return currentPackage.Item2;
				}
				else
				{
					// Otherwise return null
					return null;
				}
			}

			#endregion

			#endregion
		}
	}
}