﻿using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes that want to be called at start-up to perform initialization routine. One instance is created using
	/// <see cref="System.Activator"/> and <see cref="InitializationTypeScan(IEnumerable{Type})"/> is called using types that match
	/// <see cref="GetTypeScanPredicates"/>.
	/// </summary>
	public interface IInitializationTypeScan
    {
		#region Public methods

		/// <summary>
		/// Method called at app's startup with <see cref="Type"/>s that match <see cref="GetTypeScanPredicates"/>
		/// </summary>
		void InitializationTypeScan(IEnumerable<Type> foundTypes);

		/// <summary>
		/// Predicates that determine all types eligible for inclusion in enumeration passes as parameter to
		/// <see cref="InitializationTypeScan(IEnumerable{Type})"/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Predicate<Type>> GetTypeScanPredicates();

		#endregion
	}
}