using Autofac;
using System;
using System.Reflection;

namespace ECAT.Core
{
	/// <summary>
	/// IoC provider. Register elements either using <see cref="RegisterAsType"/> and <see cref="RegisterAsInstance"/> attributes or
	/// manually in classes deriving from <see cref="Autofac.Module"/> by overriding <see cref="Autofac.Module.Load(ContainerBuilder)"/>
	/// </summary>
	public static class IoC
	{
		#region Public static properties		
		
		/// <summary>
		/// Container with component registration
		/// </summary>
		public static IContainer Container { get; private set; }

		#endregion

		#region Private static methods

		/// <summary>
		/// Scans the assembly and registers all types marked with <see cref="RegisterAsType"/> in the <paramref name="builder"/>.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="assemblies">Can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		private static void ScanAndRegisterAssemblyTypes(this ContainerBuilder builder, Assembly[] assemblies) =>
			// Check if the builder is not null (do nothing) and if assembly is not null (throw exception) and register assembly types
			builder?.RegisterAssemblyTypes(assemblies ?? throw new ArgumentNullException(nameof(assemblies))).
			// That have RegisterAsTyhpe attribute defined
			Where((type) => Attribute.IsDefined(type, typeof(RegisterAsType))).
			// Get the attribute (it's guaranteed to be be present) and use types defined in it (they're guaranteed to not be null)
			As((type) => (Attribute.GetCustomAttribute(type, typeof(RegisterAsType)) as RegisterAsType).Types);

		/// <summary>
		/// Scans the assembly and registers all types marked with <see cref="RegisterAsType"/> in the <paramref name="builder"/>
		/// as single instances.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="assemblies">Can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		private static void ScanAndRegisterAssemblyInstances(this ContainerBuilder builder, Assembly[] assemblies) =>
			// Check if the builder is not null (do nothing) and if assembly is not null (throw exception) and register assembly types
			builder?.RegisterAssemblyTypes(assemblies ?? throw new ArgumentNullException(nameof(assemblies))).
			// That have RegisterAsTyhpe attribute defined
			Where((type) => Attribute.IsDefined(type, typeof(RegisterAsInstance))).
			// Get the attribute (it's guaranteed to be be present) and use types defined in it (they're guaranteed to not be null)
			As((type) => (Attribute.GetCustomAttribute(type, typeof(RegisterAsInstance)) as RegisterAsInstance).Types).
			// And specify single instance
			SingleInstance();

		#endregion

		#region Public static methods

		/// <summary>
		/// Builds the container, scans all assemblies in <paramref name="assemblies"/> for types marked with <see cref="RegisterAsType"/>
		/// </summary>
		/// <param name="assemblies">Assemblies to scan in search of types marked with <see cref="RegisterAsType"/></param>
		public static void Build(Assembly[] assemblies)
		{
			// Create a new builder
			var builder = new ContainerBuilder();

			// Register types
			builder.ScanAndRegisterAssemblyTypes(assemblies);

			// Register instances
			builder.ScanAndRegisterAssemblyInstances(assemblies);

			// Register modules
			builder.RegisterAssemblyModules(assemblies);

			// Build the container
			Container = builder.Build();
		}

		/// <summary>
		/// Calls and returns the result of <see cref="IoC.Container.Resolve{T}"/> (without getting a lifetime scope;
		/// should be used on singletons or possibility of not being cleaned-up is not a problem)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Resolve<T>() => Container.Resolve<T>();

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