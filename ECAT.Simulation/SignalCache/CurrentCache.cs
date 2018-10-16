using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Base class that may be used when implementing <see cref="ICurrentDB"/>, provides means of storing
	/// current signal datas along with an <see cref="ISignalInformation"/> constructed based on them.
	/// </summary>
	/// <typeparam name="TSignal"></typeparam>
	internal abstract class CurrentCache<TSignal> : SignalCache<Tuple<ITwoTerminal, bool>, TSignal>
		where TSignal : ISignalData
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="activeComponentsCurrents"></param>
		public CurrentCache(IEnumerable<KeyValuePair<int, TSignal>> activeComponentsCurrents) :
			base(new CustomEqualityComparer<Tuple<ITwoTerminal, bool>>(
				// Compare the elements of the Tuples, not tuples themselves
				(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2))
		{
			// Create a new dictionary
			_ActiveComponentsCache = new Dictionary<Tuple<int, bool>, Tuple<TSignal, ISignalInformation>>(
				// Check if parameter is not null
				activeComponentsCurrents?.
				// Project it to a tuple with the int key and bool (false, indicating no direction change) and the value
				Select((x) => new KeyValuePair<Tuple<int, bool>, TSignal>(new Tuple<int, bool>(x.Key, false), x.Value)).
				// Concat it with the same currents, this time
				Concat(activeComponentsCurrents.
				// Project them to a tuple with the int key and bool (true, indicating direction was reversed) and the value negated
				Select((x) => new KeyValuePair<Tuple<int, bool>, TSignal>(new Tuple<int, bool>(x.Key, true), CopyAndNegate(x.Value)))).
				// Finally transform it to a dictionary of required types
				ToDictionary(
				// Key stays the same
				(x) => x.Key,
				// Value is the signal and information based on it
				(x) => Tuple.Create(x.Value, IoC.Resolve<ISignalInformation>(x.Value, IoC.Resolve<ICommonSignalDescriptions>().Current)))
				// If the null check above caught a null value, this operator will result in the exception being thrown
				?? throw new ArgumentNullException(nameof(activeComponentsCurrents)));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Cache with currents produced by <see cref="IDCVoltageSource"/>s, <see cref="IACVoltageSource"/>s and <see cref="IOpAmp"/>s
		/// For key Item1 is the index of the <see cref="IActiveComponent"/>, Item2 determines whether direction was reversed.
		/// For value Item1 is the data on which the signal is based, Item2 is information about the signal data.
		/// </summary>
		protected IReadOnlyDictionary<Tuple<int, bool>, Tuple<TSignal, ISignalInformation>> _ActiveComponentsCache { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// If cache does not contain an entry with key given by <paramref name="component"/> and
		/// <paramref name="voltageBA"/>, caches <paramref name="signal"/>, otherwise doesn't do anything
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="component"></param>
		/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
		/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
		/// that direction was reversed</param>
		private void CacheHelper(TSignal signal, ITwoTerminal component, bool voltageBA)
		{
			if (!_Cache.ContainsKey(new Tuple<ITwoTerminal, bool>(component, voltageBA)))
			{
				_Cache.Add(new Tuple<ITwoTerminal, bool>(component, voltageBA),
					Tuple.Create(
						signal,
						IoC.Resolve<ISignalInformation>(signal, IoC.Resolve<ICommonSignalDescriptions>().Current)));
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Tries to obtain current from the <see cref="_ActiveComponentsCache"/>, if successful assigns it to <paramref name="current"/>
		/// and returns true, otherwise assigns default value of TSignal to <paramref name="current"/> and returns false
		/// </summary>
		/// <param name="activeComponentIndex"></param>
		/// <param name="current"></param>
		/// <param name="reverseDirection">True if the direction is assumed to be standard, false if reversed</param>
		/// <returns></returns>
		protected bool TryGetActiveComponentCurrent(int activeComponentIndex, out TSignal current, bool reverseDirection)
		{
			// Check if the current is in the cache (If it was not provided during object construction, it's not possible to
			// calculate it now)
			if (_ActiveComponentsCache.TryGetValue(
				new Tuple<int, bool>(activeComponentIndex, reverseDirection), out var currentPackage))
			{
				// If so, assign it and return success
				current = currentPackage.Item1;
				return true;
			}
			else
			{
				// Otherwise assign default value and return false
				current = default(TSignal);
				return false;
			}
		}

		/// <summary>
		/// Tries to obtain current from the <see cref="_ActiveComponentsCache"/>, if successful assigns it to <paramref name="info"/>
		/// and returns true, otherwise assigns null to <paramref name="info"/> and returns false
		/// </summary>
		/// <param name="activeComponentIndex"></param>
		/// <param name="info"></param>
		/// <param name="reverseDirection">True if the direction is assumed to be standard, false if reversed</param>
		/// <returns></returns>
		protected bool TryGetActiveComponentCurrent(int activeComponentIndex, out ISignalInformation info, bool reverseDirection)
		{
			// Check if the current is in the cache (If it was not provided during object construction, it's not possible to
			// calculate it now)
			if (_ActiveComponentsCache.TryGetValue(
				new Tuple<int, bool>(activeComponentIndex, reverseDirection), out var currentPackage))
			{
				// If so, assign it and return success
				info = currentPackage.Item2;
				return true;
			}
			else
			{
				// Otherwise assign null and return false
				info = null;
				return false;
			}
		}

		/// <summary>
		/// Caches the <paramref name="signal"/> as well as its negated copy (with inverted indexes)
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="component"></param>
		/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
		/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
		/// that direction was reversed</param>
		protected void CacheCurrent(TSignal signal, ITwoTerminal component, bool voltageBA)
		{
			// Cache the original
			CacheHelper(signal, component, voltageBA);

			// And cache the reversed one
			CacheHelper(CopyAndNegate(signal), component, !voltageBA);
		}

		/// <summary>
		/// Tries to construct a current for <paramref name="component"/>, returns true on success
		/// </summary>
		/// <param name="component"></param>
		/// <param name="voltageBA">If true, it means that current was calculated for voltage drop from
		/// <see cref="ITwoTerminal.TerminalA"/> (reference) to <see cref="ITwoTerminal.TerminalB"/>, if false it means that
		/// that direction was reversed</param>
		/// <param name="current">Current constructed if successful, null otherwise</param>
		/// <returns></returns>
		protected abstract bool TryConstructCurrent(ITwoTerminal component, bool voltageBA, out TSignal current);

		/// <summary>
		/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		protected abstract TSignal CopyAndNegate(TSignal signal);

		/// <summary>
		/// Checks if current through <paramref name="component"/> can be obtained from cache, if not performs
		/// all possible actions to create it and cache it. Returns true if, at the end of the method call, the current may be
		/// obtained from <see cref="_PassiveCurrentsCache"/>, false otherwise.
		/// </summary>
		/// <param name="component">Element for which the current is considered</param>
		/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
		/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
		/// <returns></returns>
		protected bool TryEnableCurrent(ITwoTerminal component, bool voltageBA)
		{
			// If there was a cached entry already return it
			if(_Cache.ContainsKey(new Tuple<ITwoTerminal, bool>(component, voltageBA)))
			{
				return true;
			}
			// Try to construct an entry
			else if(TryConstructCurrent(component, voltageBA, out var current))
			{
				CacheCurrent(current, component, voltageBA);
				return true;
			}

			return false;
		}

		#endregion
	}	
}