using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;

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
		public CurrentCache() : base(new CustomEqualityComparer<Tuple<ITwoTerminal, bool>>(
				// Compare the elements of the Tuples, not tuples themselves
				(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2)) { }

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
		/// <returns></returns>
		protected abstract bool TryConstructCurrent(ITwoTerminal component, bool voltageBA);

		/// <summary>
		/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		protected abstract TSignal CopyAndNegate(TSignal signal);

		/// <summary>
		/// Checks if current through <paramref name="element"/> can be obtained from cache, if not performs
		/// all possible actions to create it and cache it. Returns true if, at the end of the method call, the current may be
		/// obtained from <see cref="_PassiveCurrentsCache"/>, false otherwise.
		/// </summary>
		/// <param name="element">Element for which the current is considered</param>
		/// <param name="voltageBA">If true, voltage used to calculate the current is taken from <see cref="ITwoTerminal.TerminalA"/>
		/// (reference node) to <see cref="ITwoTerminal.TerminalB"/>, if false the direction is reversed</param>
		/// <returns></returns>
		protected bool TryEnableCurrent(ITwoTerminal element, bool voltageBA) =>
			// If there was a cached entry already return it
			_Cache.ContainsKey(new Tuple<ITwoTerminal, bool>(element, voltageBA)) || TryConstructCurrent(element, voltageBA);

		#endregion
	}
	
}