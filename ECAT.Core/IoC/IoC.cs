using Autofac;
using CSharpEnhanced.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ECAT.Core
{
	/// <summary>
	/// IoC provider. Register elements either using <see cref="RegisterAsType"/> and <see cref="RegisterAsInstance"/> attributes or
	/// manually in classes implementing from <see cref="IIoCRegistartionModule"/>. Availabilty is determined by presence of
	/// <see cref="ConstructorDeclaration"/> parameter on each interface. Every service is guaranteed to be resolvable
	/// without any parameters unless <see cref="ConstructorDeclaration"/>s are specified and a parameterless
	/// <see cref="ConstructorDeclaration"/> is not one of them (similarly to standard constructors).
	/// </summary>
	public static class IoC
	{
		#region Private static properties

		/// <summary>
		/// True if the <see cref="Container"/> was already built
		/// </summary>
		private static bool IsBuilt => Container != null;

		#endregion

		#region Public static properties		

		/// <summary>
		/// Container with component registration
		/// </summary>
		public static IContainer Container { get; private set; }

		#endregion

		#region Private static methods

		/// <summary>
		/// Registers types with <see cref="RegisterAsType"/> attribute with <paramref name="builder"/>
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="types"></param>
		private static void RegisterTypes(this ContainerBuilder builder, IEnumerable<Type> types) => types.
			// Find all types with RegisterAsType attribute
			Where((type) => Attribute.IsDefined(type, typeof(RegisterAsType))).
			// For each type
			ForEach((type) =>
				// Register it
				builder.RegisterType(type).
				// As types defined in attribute
				As((Attribute.GetCustomAttribute(type, typeof(RegisterAsType)) as RegisterAsType).Types));

		/// <summary>
		/// Registers types with <see cref="RegisterAsInstance"/> attribute with <paramref name="builder"/> as single instances
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="types"></param>
		private static void RegisterInstances(this ContainerBuilder builder, IEnumerable<Type> types) => types.
			// Find all types with RegisterAsInstance attribute
			Where((type) => Attribute.IsDefined(type, typeof(RegisterAsInstance))).
			// For each type
			ForEach((type) =>
				// Register it
				builder.RegisterType(type).
				// As types defined in attribute
				As((Attribute.GetCustomAttribute(type, typeof(RegisterAsInstance)) as RegisterAsInstance).Types).
				// As single instances
				SingleInstance());

		/// <summary>
		/// Finds all <see cref="IIoCRegistartionModule"/>s, creates an instance of each and calls its
		/// <see cref="IIoCRegistartionModule.Register(ContainerBuilder)"/> methods
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="types"></param>
		private static void RegisterModules(this ContainerBuilder builder, IEnumerable<Type> types) => types.
			// Find all types implementing IIoCRegistartionModule
			Where((type) => typeof(IIoCRegistartionModule).IsAssignableFrom(type)).
			// Filter out those that Activator won't be able to construct
			Where((type) => type.CanBeConstructedWithoutParameters()).
			// Create an instance of each
			Select((type) => Activator.CreateInstance(type)).
			// Cast it to IIoCRegistartionModule
			Cast<IIoCRegistartionModule>().
			// Call Register method on each
			ForEach((module) => module.Register(builder));

		/// <summary>
		/// Checks whether all types marked with <see cref="NecessaryService"/> attribute are available in the
		/// <paramref name="container"/>
		/// </summary>
		/// <param name="container"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		private static IEnumerable<Type> FindUnregisteredServices(this IContainer container, IEnumerable<Type> types) => types.
			// Get all types that notify that they want to be registered
			Where((type) => Attribute.IsDefined(type, typeof(NecessaryService))).
			// And check if each of them is registered
			Where((type) => !container.IsRegistered(type));
		
		/// <summary>
		/// Gets <see cref="ConstructorDeclaration"/>s from <paramref name="type"/>. Treats no declarations as a declaration of a
		/// parameterless constructor. Projects the attributes to an enumeration (each element is a different constructor declaration)
		/// of type enumerations (which corresponds to types of parameters for the constructor).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<IEnumerable<Type>> GetConstructorDeclarations(Type type) =>
			// Check if the attribute is defined
			Attribute.IsDefined(type, typeof(ConstructorDeclaration)) ?
			// If so get all its instances
			Attribute.GetCustomAttributes(type, typeof(ConstructorDeclaration)).
			// Cast them to the proper type
			Cast<ConstructorDeclaration>().
			// Get their parameter types
			Select((attribute) => attribute.Parameters) :
			// If the parameter was not defined return a one-element array of an empty type enumeration
			new IEnumerable<Type>[] { Enumerable.Empty<Type>() };

		/// <summary>
		/// Returns true if <paramref name="type"/> has a public constructor with given parameters
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static bool HasPublicConstructor(this Type type, IEnumerable<Type> parameters)
		{
			// Try to get the matching constructor
			var constructor = type.GetMatchingConstructor(parameters.ToArray());

			// Return true if it was found (it's not null) and if it's public
			return constructor != null && constructor.IsPublic;
		}

		/// <summary>
		/// Returns a sequence of KeyValuePairs where key is a type which implements some services but doesn't implement all
		/// constructors requested by them, value is a sequence of services that don't have the requested constructors implemented.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		private static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindServicesWithoutImplementedConstructors(
			this IEnumerable<Type> types) => types.
			// Get types that wanted to be registered as type
			Where((type) => Attribute.IsDefined(type, typeof(RegisterAsType))).
			// Project them to a dictionary
			ToDictionary(
				// Key is the type
				(type) => type,
				// Value is chosen as follows: Get the services the type wants to register as
				(type) => (Attribute.GetCustomAttribute(type, typeof(RegisterAsType)) as RegisterAsType).Types.
					// And get only those services whose at least one speicifed constructor isn't available
					Where((service) => !GetConstructorDeclarations(service).
						// Check each declaration using a helper method
						All((declaration) => type.HasPublicConstructor(declaration)))).
			// Finally filter out those entries from the dictionary that are false positives (their value sequence has no elements)
			// They appear because if type implements all constructors for its all services then the filtering will return an empty
			// sequence rather than omit it entirely
			Where((x) => x.Value.Count() > 0);


		/// <summary>
		/// Checks whether all <paramref name="types"/> marked with <see cref="NecessaryService"/> attribute are available in the
		/// <paramref name="container"/>, if not throws an exception with detailed information.
		/// Checks whether all types implement all requested constructors from their services, throws an exception if it's not the case.
		/// </summary>
		/// <param name="container"></param>
		/// <exception cref="ServicesUnregisteredException"></exception>
		[Conditional("DEBUG")]
		private static void CheckContainerIntegrity(this IContainer container, IEnumerable<Type> types)
		{
			// Check if all services were registered
			var unregisteredServices = Container.FindUnregisteredServices(types);

			if (unregisteredServices.Count() > 0)
			{
				throw new ServicesUnregisteredException(unregisteredServices);
			}

			// Check if all constructors were created
			var typesMissingConstructors = FindServicesWithoutImplementedConstructors(types);

			if(typesMissingConstructors.Count() > 0)
			{
				throw new ServicesWithMissingConstructorsException(typesMissingConstructors);
			}
		}

		#endregion

		#region Public static methods
		
		/// <summary>
		/// Builds the container, finds all types in <paramref name="types"/> marked with <see cref="RegisterAsType"/> and
		/// <see cref="RegisterAsInstance"/> and registers them. Can only be built once. Each subsequent call to this
		/// method won't do anything.
		/// </summary>
		/// <param name="types">All types defined in ECAT assemblies that will be used in the app</param>
		/// <exception cref="ArgumentNullException"></exception>
		internal static void Build(IEnumerable<Type> types)
		{
			// Check if the container wasn't already built
			if (IsBuilt)
			{
				return;
			}
		
			// Create a new builder
			var builder = new ContainerBuilder();

			// Register modules
			builder.RegisterModules(types);

			// Register types
			builder.RegisterTypes(types);

			// Register instances
			builder.RegisterInstances(types);

			// Build the container
			Container = builder.Build();

			// Check integrity (if all types are registered, etc.)
			Container.CheckContainerIntegrity(types);
		}

		/// <summary>
		/// Calls and returns the result of <see cref="IoC.Container.Resolve{T}"/> (without getting a lifetime scope;
		/// should be used on singletons or possibility of not being cleaned-up is not a problem)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Resolve<T>() => Container.Resolve<T>();

		/// <summary>
		/// Shortcut to BeginLifetimeScope method on <see cref="Container"/>
		/// </summary>
		/// <returns></returns>
		public static ILifetimeScope Scope() => Container.BeginLifetimeScope();

		/// <summary>
		/// Shortcut to log messages through <see cref="IInfoLogger"/>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="loggerID">ID used to identify the caller. Messages may be removed only if the same ID is used later</param>
		/// <param name="duration">Maximum lifetime of the message, if the message was still presented after the time expires,
		/// it will be removed. By default it's infinite</param>
		public static void Log(string message, int loggerID, InfoLoggerMessageDuration duration = InfoLoggerMessageDuration.Infinite) =>
			Container.Resolve<IInfoLogger>().Log(message, loggerID, duration);

		/// <summary>
		/// Shortcut to log anonymous messages through <see cref="IInfoLogger"/>
		/// </summary>
		/// <param name="message"></param>		
		/// <param name="duration">Maximum lifetime of the message, if the message was still presented after the time expires,
		/// it will be removed. By default it's infinite</param>
		public static void Log(string message, InfoLoggerMessageDuration duration = InfoLoggerMessageDuration.Infinite) =>
			Container.Resolve<IInfoLogger>().Log(message, duration);

		/// <summary>
		/// Shortcut to remove logged messages through <see cref="IInfoLogger"/>
		/// </summary>
		/// <param name="loggerID"></param>
		public static void RemoveLog(int loggerID) => Container.Resolve<IInfoLogger>().RemoveLog(loggerID);

		#endregion
	}
}