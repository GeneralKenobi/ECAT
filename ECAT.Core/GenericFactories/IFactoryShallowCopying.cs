﻿namespace ECAT.Core
{
	/// <summary>
	/// Interface for factories allowing for construction of an instance of <see cref="T"/> by making a shallow copy of another instance
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFactoryShallowCopying<T>
    {
		#region Methods

		/// <summary>
		/// Constructs a shallow copy of <paramref name="source"/>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		T Construct(T source);

		#endregion
	}
}