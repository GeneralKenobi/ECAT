using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Core
{
	/// <summary>
	/// Exception thrown when an interface marked with <see cref="NecessaryService"/> attribute was not registered with <see cref="IoC"/>
	/// </summary>
	internal class ServicesUnregisteredException : Exception
    {
		#region Constructors

		/// <summary>
		/// Default constructor, requires parameter
		/// </summary>
		/// <param name="unregisteredServices">Services that were not registered with IoC, can't be null or empty</param>
		public ServicesUnregisteredException(IEnumerable<Type> unregisteredServices)
		{
			if(unregisteredServices == null || unregisteredServices.Count() == 0)
			{
				throw new ArgumentException(nameof(unregisteredServices) + " can't be null or empty");
			}

			UnregisteredServices = unregisteredServices;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Services that were not registered with IoC
		/// </summary>
		public IEnumerable<Type> UnregisteredServices { get; }

		/// <summary>
		/// The error message that explains the reason for the exception
		/// </summary>
		public override string Message { get; } = "Not all interfaces marked with " + nameof(NecessaryService) +
			" were registered with IoC, check " + nameof(UnregisteredServices) + " for list of missing services";

		#endregion
	}
}