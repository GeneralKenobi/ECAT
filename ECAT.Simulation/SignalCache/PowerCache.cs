using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	/// <summary>
	/// Base class that may be used when implementing <see cref="IPowerDB"/>, provides means of storing
	/// power signal datas along with an <see cref="ISignalInformation"/> constructed based on them.
	/// </summary>
	/// <typeparam name="TSignal"></typeparam>
	internal abstract class PowerCache<TSignal> : SignalCache<IBaseComponent, TSignal>
		where TSignal : ISignalData
	{
		#region Protected methods

		/// <summary>
		/// Caches the <paramref name="power"/> in <see cref="_Cache"/>
		/// </summary>
		/// <param name="component">Component for which the current flow is considered</param>
		/// <param name="power"></param>
		protected void CachePower(TSignal power, IBaseComponent component) =>
			_Cache.Add(component, Tuple.Create(
				power, IoC.Resolve<ISignalInformation>(power, IoC.Resolve<ICommonSignalDescriptions>().Power)));

		/// <summary>
		/// Tries to construct a power for <paramref name="component"/>, returns true on success
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected abstract bool TryConstructPower(IBaseComponent component, out TSignal power);

		/// <summary>
		/// Returns true if power for <paramref name="component"/> can be obtained from <see cref="_Cache"/>
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		protected bool TryEnablePower(IBaseComponent component)
		{
			// Check if the cache already contains an entry, otherwise try to construct it
			if (_Cache.ContainsKey(component))
			{
				return true;
			}
			// Try to construct it
			else if(TryConstructPower(component, out var power))
			{
				CachePower(power, component);
				return true;
			}

			return false;
		}

		#endregion
	}	
}