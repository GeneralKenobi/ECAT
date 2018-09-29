using Autofac;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using ECAT.DataDisplay;
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
			typeof(Core.DummyType),
			typeof(DataDisplay.DummyType),
			typeof(Design.DummyType),
			typeof(Simulation.DummyType),
			typeof(ViewModel.DummyType),
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

		/// <summary>
		/// Returns all types defined in <paramref name="assemblies"/>
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		private static IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies)
		{
			// Start with an empty sequence
			var types = Enumerable.Empty<Type>();

			foreach (var item in assemblies)
			{
				// Add types from each assembly to it
				types = types.Concat(item.DefinedTypes);
			}

			// Return it
			return types;
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Injects dependencies to <see cref="ECAT.Core.IoC"/>. Should be invoked as soon as possible
		/// </summary>
		public static void Run()
		{
			// Get assemblies, get types defined in them and run the initialization in Core
			Core.Initialization.Run(GetTypes(GetECATAssemblies()));

			// Remove the TypeRefArray from references
			TypeRefArray = null;
		}

		#endregion
	}
}