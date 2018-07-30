﻿using Autofac;
using System;

namespace ECAT.Core
{
	/// <summary>
	/// IoC provider
	/// </summary>
	public static class IoC
	{
		#region Public methods

		/// <summary>
		/// Builds the container for the given <see cref="ContainerBuilder"/>
		/// </summary>
		/// <param name="builder"></param>
		public static void Build(ContainerBuilder builder)
		{
			var container = builder.Build();

			if (NecessaryElementsRegistered(container))
			{
				Container = container;
			}
			else
			{
				throw new Exception("The builder container does not have all necessary interfaces registered");
			}
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
		/// <param name="loggerID"></param>
		public static void Log(string message, int loggerID) => Container.Resolve<IInfoLogger>().Log(message);

		#endregion

		#region Private static methods

		/// <summary>
		/// Returns true if all the main (necessary) interfaces for the app are registered
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		private static bool NecessaryElementsRegistered(IContainer container) => container.IsRegistered<IDesignManager>();

		#endregion

		#region Public static properties		

		/// <summary>
		/// Indicates whether the IoC was setup
		/// </summary>
		public static bool Ready => Container == null;

		/// <summary>
		/// Container with component registration
		/// </summary>
		public static IContainer Container { get; private set; } = null;

		#endregion
	}
}