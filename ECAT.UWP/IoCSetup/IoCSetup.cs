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
		#region Public static methods

		/// <summary>
		/// Injects dependencies to <see cref="ECAT.Core.IoC"/>. Should be invoked at the beginning 
		/// </summary>
		public static void Run()
		{
			// Create the builder
			ContainerBuilder builder = new ContainerBuilder();

			// Register necessary types
			builder.RegisterInstance(new DesignManager()).As<IDesignManager>();
			builder.RegisterInstance(new SimulationManager()).As<ISimulationManager>();			
			builder.RegisterInstance(new ComponentFactory()).As<IComponentFactory>();
			builder.RegisterInstance(new PlanePositionFactory()).As<IPlanePositionFactory>();
			builder.RegisterInstance(new DefaultValues()).As<IDefaultValues>();
			builder.RegisterInstance(new InfoLogger()).As<IInfoLogger>();


			IoC.Build(builder);
		}

		#endregion
	}
}