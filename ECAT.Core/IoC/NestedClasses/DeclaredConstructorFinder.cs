using Autofac.Core.Activators.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace ECAT.Core
{
	partial class IoC
	{
		/// <summary>
		/// Class used to find constructors on types that match constructor declarations on service that type is registered as.
		/// </summary>
		private class DeclaredConstructorFinder : IConstructorFinder
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="service"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public DeclaredConstructorFinder(Type service)
			{
				Service = service ?? throw new ArgumentNullException(nameof(service));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Service as which the type is being registered
			/// </summary>
			private Type Service { get; }

			#endregion

			#region Public methods

			/// <summary>
			/// Finds constructors that match those declared using <see cref="ConstructorDeclaration"/> attribute on <see cref="Service"/>
			/// </summary>
			/// <param name="targetType"></param>
			/// <returns></returns>
			public ConstructorInfo[] FindConstructors(Type targetType)
			{
				var declarations = GetConstructorDeclarations(Service);

				// Get all constructors
				return targetType.GetConstructors().
				// Filter them, get constructor declarations
				Where((constructor) => declarations.
					// Check if any declaration matches the constructor by getting the constructor's parameters
					Any((declaration) => constructor.GetParameters().
						// Projecting them to their types
						Select((parameter) => parameter.ParameterType).
					// And comparing both sequences
					SequenceEqual(declaration))).
				// Finally return the result as an array
				ToArray();
			}

			#endregion
		}
	}
}