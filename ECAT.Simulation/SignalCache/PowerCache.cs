using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	/// <summary>
	/// Base class that may be used when implementing <see cref="IPowerDB"/>, provides means of storing
	/// power signal datas along with an <see cref="ISignalInformation"/> constructed based on them.
	/// </summary>
	/// <typeparam name="TSignal"></typeparam>
	internal abstract class PowerCache<TSignal> : SignalCache<Tuple<IBaseComponent, bool>, TSignal>
		where TSignal : ISignalData
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public PowerCache() : base(new CustomEqualityComparer<Tuple<IBaseComponent, bool>>(
			(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2))
		{
			
		}

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
		private void CacheHelper(TSignal signal, IBaseComponent component, bool voltageBA)
		{
			if (!_Cache.ContainsKey(new Tuple<IBaseComponent, bool>(component, voltageBA)))
			{
				_Cache.Add(new Tuple<IBaseComponent, bool>(component, voltageBA),
					Tuple.Create(
						signal,
						IoC.Resolve<ISignalInformation>(signal, IoC.Resolve<ICommonSignalDescriptions>().Current)));
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Caches the <paramref name="power"/> in <see cref="_Cache"/>
		/// </summary>
		/// <param name="component">Component for which the current flow is considered</param>
		/// <param name="power"></param>
		protected void CachePower(TSignal power, IBaseComponent component, bool voltageBA)
		{
			CacheHelper(power, component, voltageBA);
		}

		/// <summary>
		/// Tries to construct a power for <paramref name="component"/>, returns true on success
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected abstract bool TryConstructPower(IBaseComponent component, out TSignal power, bool voltageBA);

		/// <summary>
		/// Returns true if power for <paramref name="component"/> can be obtained from <see cref="_Cache"/>
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		protected bool TryEnablePower(IBaseComponent component, bool voltageBA)
		{
			// Check if the cache already contains an entry, otherwise try to construct it
			if (_Cache.ContainsKey(Tuple.Create(component, voltageBA)))
			{
				return true;
			}
			// Try to construct it
			else if(TryConstructPower(component, out var power, voltageBA))
			{
				CachePower(power, component, voltageBA);
				return true;
			}

			return false;
		}

		#endregion
	}	
}