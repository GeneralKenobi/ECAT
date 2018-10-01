using Autofac;
using CSharpEnhanced.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Core
{
	/// <summary>
	/// IoC provider. Register elements either using <see cref="RegisterAsType"/> and <see cref="RegisterAsInstance"/> attributes or
	/// manually in classes implementing from <see cref="IIoCRegistartionModule"/>. All registered classes should have a public,
	/// parameterless constructor (factory pattern is strongly recommended for construction with parameters).
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
		/// Checks whether all <paramref name="types"/> marked with <see cref="NecessaryService"/> attribute are available in the
		/// <paramref name="container"/>, if not throws an exception with detailed information.
		/// </summary>
		/// <param name="container"></param>
		/// <exception cref="ServicesUnregisteredException"></exception>
		private static void CheckContainerIntegrity(this IContainer container, IEnumerable<Type> types)
		{
			var unregisteredServices = Container.FindUnregisteredServices(types);

			if (unregisteredServices.Count() > 0)
			{
				throw new ServicesUnregisteredException(unregisteredServices);
			}
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Builds the container, scans all assemblies in <paramref name="assemblies"/> for types marked with
		/// <see cref="RegisterAsType"/> and <see cref="RegisterAsInstance"/>. Can only be built once. Each subsequent call to this
		/// method won't do anything.
		/// </summary>
		/// <param name="assemblies">Assemblies to scan in search of types marked with <see cref="RegisterAsType"/> and
		/// <see cref="RegisterAsInstance"/></param>
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