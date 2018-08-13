using Autofac;
using ECAT.Core;
using ECAT.Design;
using ECAT.Simulation;

namespace ECAT.UWP
{
	/// <summary>
	/// Class that contains a method that prepares the IoC container for use
	/// </summary>
	public static class IoCSetup
	{
		#region Private static methods

		/// <summary>
		/// Registers <see cref="IManager.GetTypesToRegister"/> as types and <see cref="IManager.GetInstancesToRegister"/> as instances
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="manager"></param>
		private static void RegisterManagersImplementations(ContainerBuilder builder, IManager manager)
		{
			// Register instances
			foreach(var instance in manager.GetInstancesToRegister())
			{
				builder.RegisterInstance(instance.Item2).As(instance.Item1);
			}

			// Register types
			foreach(var type in manager.GetTypesToRegister())
			{
				builder.RegisterType(type.Item2).As(type.Item1);
			}
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Injects dependencies to <see cref="ECAT.Core.IoC"/>. Should be invoked at the beginning 
		/// </summary>
		public static void Run()
		{
			// Create the builder
			ContainerBuilder builder = new ContainerBuilder();

			// Create managers
			var designManager = new DesignManager();
			var simulationManager = new SimulationManager();

			// Register them
			builder.RegisterInstance(designManager).As<IDesignManager>();
			builder.RegisterInstance(simulationManager).As<ISimulationManager>();

			// Let them make their registrations
			RegisterManagersImplementations(builder, designManager);
			RegisterManagersImplementations(builder, simulationManager);

			IoC.Build(builder);
		}

		#endregion
	}
}