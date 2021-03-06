﻿using ECAT.Core;
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
		private class BiasCurrent : CurrentCache<IPhasorDomainSignal>, ICurrentDB, ICurrentSignalDB<IPhasorDomainSignal>
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires two parameters, if either is null, an exception will be thrown
			/// </summary>
			/// <param name="voltageDrops">Object contain information about voltage drops calculated in simulation, can't be null</param>
			/// <param name="activeComponentsCurrents">Currents produced by active components, can't be null</param>
			public BiasCurrent(IVoltageSignalDB<IPhasorDomainSignal> voltageDrops,
				IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> activeComponentsCurrents) :
				base(activeComponentsCurrents)
			{
				_VoltageDrops = voltageDrops ?? throw new ArgumentNullException(nameof(voltageDrops));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Contains information about voltage drops calculated in simulation
			/// </summary>
			private IVoltageSignalDB<IPhasorDomainSignal> _VoltageDrops { get; }

			#endregion

			#region Private methods

			///// <summary>
			///// Returns a DC current flowing through a two terminal
			///// </summary>
			///// <param name="voltageDrop"></param>
			///// <param name="twoTerminal"></param>
			///// <returns></returns>
			//private double GetPassiveTwoTerminalDCCurrent(IPhasorDomainSignal voltageDrop, ITwoTerminal twoTerminal) =>
			//	twoTerminal.GetConductance() * voltageDrop.Phasors.
			//	Where((x) => x.Key.FrequencyCategory == FrequencyCategory.DC).
			//	Sum((x) => x.Value.Real) ;
			//
			///// <summary>
			///// Returns AC current phasors for a two terminal
			///// </summary>
			///// <param name="voltageDrop"></param>
			///// <param name="twoTerminal"></param>
			///// <returns></returns>
			//private IEnumerable<KeyValuePair<double, Complex>> GetPassiveTwoTerminalACCurrentPhasors(
			//	IPhasorDomainSignal voltageDrop, ITwoTerminal twoTerminal) =>
			//	voltageDrop.Phasors.Select((phasor) =>
			//	new KeyValuePair<double, Complex>(phasor.Key, phasor.Value * twoTerminal.GetAdmittance(phasor.Key)));

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
			protected override bool TryConstructCurrent(ITwoTerminal element, bool voltageBA, out IPhasorDomainSignal current)
			{
				// Try to get voltage drop across the element
				if (_VoltageDrops.TryGet(element, out var voltageDrop, voltageBA))
				{
					// If successful, create a new current signal based on it, cache it
					//current = IoC.Resolve<IPhasorDomainSignal>(
					//		GetPassiveTwoTerminalDCCurrent(voltageDrop, element),
					//		GetPassiveTwoTerminalACCurrentPhasors(voltageDrop, element));

					current = IoC.Resolve<IPhasorDomainSignal>(
						voltageDrop.Phasors.Select((x) => x.Value * element.GetAdmittance(x.Key.Frequency)));

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
			protected override IPhasorDomainSignal CopyAndNegate(IPhasorDomainSignal signal) => signal.CopyAndNegate();

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
			public bool TryGet(IResistor resistor, out IPhasorDomainSignal current, bool voltageBA = true) =>
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
			public bool TryGet(ICapacitor capacitor, out IPhasorDomainSignal current, bool voltageBA = true) =>
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
			public bool TryGet(int activeComponentIndex, out IPhasorDomainSignal current, bool reverseDirection = false) =>
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