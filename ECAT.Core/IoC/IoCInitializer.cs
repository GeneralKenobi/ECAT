using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Finds types that are used for IoC registration and calls <see cref="IoC.Build(IEnumerable{Type})"/>
	/// </summary>
	public class IoCInitializer : IInitializationTypeScan
	{
		#region Public methods

		/// <summary>
		/// Returns all types that will be used for IoC registration
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Predicate<Type>> GetTypeScanPredicates() => new Predicate<Type>[]
		{
			// All types that are marked with an IoCRegistration attribute are valid
			(type) => Attribute.IsDefined(type, typeof(IoCRegistration)),
			// All types that implement IIoCRegistrationModule are valid
			(type) => typeof(IIoCRegistartionModule).IsAssignableFrom(type),
		};

		/// <summary>
		/// Passes the valid types to <see cref="IoC.Build(IEnumerable{Type})"/>
		/// </summary>
		/// <param name="foundTypes"></param>
		public void InitializationTypeScan(IEnumerable<Type> foundTypes) => IoC.Build(foundTypes);

		#endregion
	}
}