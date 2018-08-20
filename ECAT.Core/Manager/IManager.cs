using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interfaces that implement this interface are considered to be the "boss" of their project, during IoC setup they are first
	/// created and registered, then
	/// </summary>
	public interface IManager
    {
		#region Methods

		/// <summary>
		/// Returns an enumeration of types to register in IoC. Item1 is the interface to register as, Item2 is the object to register
		/// as an instance.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Tuple<Type, object>> GetInstancesToRegister();

		/// <summary>
		/// Returns an enumeration of types to register in IoC. Item1 is the interface to register as, Item2 is the type to register
		/// as implementation of the interface
		/// </summary>
		/// <returns></returns>
		IEnumerable<Tuple<Type, Type>> GetTypesToRegister();

		#endregion
	}
}