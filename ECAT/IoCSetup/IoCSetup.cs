using Autofac;
using ECAT.Core;
using ECAT.Design;
using ECAT.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAT.UWP
{
	public static class IoCSetup
	{
		/// <summary>
		/// Injects dependencies to <see cref="ECAT.Core.IoC"/>. Should be invoked at the beginning 
		/// </summary>
		public static void Run()
		{
			// Create the builder
			ContainerBuilder builder = new ContainerBuilder();

			// Register necessary types
			builder.RegisterType<DesignManager>().As<IDesignManager>();

			IoC.Build(builder);

			var vm = new DesignViewModel();
			vm.Dispose();
			vm = null;
			GC.Collect();
		}
	}
}
