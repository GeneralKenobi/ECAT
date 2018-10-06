using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Base class for caches of specific 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TSignal"></typeparam>
	internal abstract class SignalCache<TKey, TSignal> where TSignal : ISignalData
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		protected SignalCache()
		{
			_Cache = new Dictionary<TKey, Tuple<TSignal, ISignalInformation>>();
		}

		/// <summary>
		/// Creates <see cref="_Cache"/> using the given equality compararer
		/// </summary>
		/// <param name="equalityComparer"></param>
		/// <exception cref="ArgumentNullException"></exception>
		protected SignalCache(IEqualityComparer<TKey> equalityComparer)
		{
			_Cache = new Dictionary<TKey, Tuple<TSignal, ISignalInformation>>(
				equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer)));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Dictionary holding signals. First item in tuple is a signal, second is an information built based on this signal.
		/// </summary>
		protected Dictionary<TKey, Tuple<TSignal, ISignalInformation>> _Cache { get; }

		#endregion
	}
}