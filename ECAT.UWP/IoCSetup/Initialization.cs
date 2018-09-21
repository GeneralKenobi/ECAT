using Autofac;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using ECAT.Design;
using ECAT.Simulation;
using ECAT.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECAT.UWP
{
	/// <summary>
	/// Class responsible for calling <see cref="Core.Initialization.Run(IEnumerable{Type})"/>, <see cref="Run"/> method should be
	/// called as soon as possible.
	/// </summary>
	public static class Initialization
	{
		#region Private static properties

		/// <summary>
		/// Array with one type from each referenced project. It is necessary to have at least one in-code reference to the project
		/// so that its assembly will be found (and not optimized out) when calling <see cref="Assembly.GetReferencedAssemblies"/>.
		/// List a type from each project here.
		/// </summary>
		private static Type[] TypeRefArray { get; set; } = new Type[]
		{
			typeof(IDesignManager),
			typeof(AppViewModel),
			typeof(DesignManager),
			typeof(SimulationManager),
		};

		#endregion

		#region Private static methods

		/// <summary>
		/// Gets all referenced assemblies (and self) that have ECAT in their name
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<Assembly> GetECATAssemblies() =>
			// Get referenced AssemblyNames
			Assembly.GetExecutingAssembly().GetReferencedAssemblies().
			// Filter them to get only those with ECAT in their name
			Where((assemblyName) => assemblyName.Name.Contains("ECAT")).
			// Project them to Assemblies by loading them (if they were already loaded then nothing will happen)
			Select((assemblyName) => Assembly.Load(assemblyName)).
			// Finally add self to the enumeration
			Concat(Assembly.GetExecutingAssembly());

		#endregion

		#region Public static methods

		/// <summary>
		/// Injects dependencies to <see cref="ECAT.Core.IoC"/>. Should be invoked as soon as possible
		/// </summary>
		public static void Run()
		{
			// Build the IoC
			IoC.Build(GetECATAssemblies().ToArray());
			var assemblies = GetECATAssemblies();
			//var publicTypes = assemblies.First().ExportedTypes.Where((type) => type.IsNe)
			// Remove the TypeRefArray from references
			TypeRefArray = null;
		}

		#endregion
	}
}