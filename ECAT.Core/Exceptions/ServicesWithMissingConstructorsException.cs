using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Exception thrown at start-up if a service declaring a constructor with <see cref="ConstructorDeclaration"/> attribute is
	/// implemented by a class not providing such constructor.
	/// </summary>
	public class ServicesWithMissingConstructorsException : Exception
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="incompleteTypes"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public ServicesWithMissingConstructorsException(IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> incompleteTypes)
		{
			IncompleteTypes = incompleteTypes ?? throw new ArgumentNullException(nameof(incompleteTypes));
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Message explaining the reason for the exception
		/// </summary>
		public override string Message { get; } = "Some service providers don't provide all requested constructors." +
			" See " + nameof(IncompleteTypes) + " for a list of incomplete providers";

		/// <summary>
		/// Sequence of KeyValuePairs where key is a type which implements some services but doesn't implement all
		/// constructors requested by them, value is a sequence of services that don't have the requested constructors implemented.
		/// </summary>
		public IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> IncompleteTypes { get; }

		#endregion
	}
}