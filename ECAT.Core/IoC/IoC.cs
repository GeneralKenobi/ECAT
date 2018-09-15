using Autofac;

namespace ECAT.Core
{
	/// <summary>
	/// IoC provider
	/// </summary>
	public static class IoC
	{
		#region Public static properties		
		
		/// <summary>
		/// Container with component registration
		/// </summary>
		public static IContainer Container { get; private set; } = null;

		#endregion

		#region Public static methods

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