using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	/// <summary>
	/// Provides functionality connected with storing, calculating and exposing power information in
	/// form of <see cref="ISignalInformation"/>
	/// </summary>
	internal abstract class PowerCache<TSignal> : SignalCache<IBaseComponent, TSignal>
		where TSignal : ISignalData
	{
		#region Private methods

		/// <summary>
		/// Caches the <paramref name="power"/> in <see cref="_Cache"/>
		/// </summary>
		/// <param name="component">Component for which the current flow is considered</param>
		/// <param name="power"></param>
		protected void CachePower(IBaseComponent component, TSignal power) =>
			_Cache.Add(component, Tuple.Create(
				power, IoC.Resolve<ISignalInformation>(power, IoC.Resolve<ICommonSignalDescriptions>().Power)));

		/// <summary>
		/// Tries to construct a power for <paramref name="component"/>, returns true on success
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected abstract bool TryConstructPower(IBaseComponent component);

		/// <summary>
		/// Returns true if power for <paramref name="component"/> can be obtained from <see cref="_Cache"/>
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		protected bool TryEnablePower(IBaseComponent component) =>
			// Check if the cache already contains an entry, otherwise try to construct it
			_Cache.ContainsKey(component) || TryConstructPower(component);

		#endregion
	}	
}