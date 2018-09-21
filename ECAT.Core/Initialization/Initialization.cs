using CSharpEnhanced.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Core
{
	/// <summary>
	/// Performs all actions that are necessary on app's start-up
	/// </summary>
	public static class Initialization
    {
		#region Private static methods

		/// <summary>
		/// Finds all types implementing <see cref="IInitializationRoutine"/>, creates an instance of them and calls
		/// <see cref="IInitializationRoutine.InitializationRoutine"/>
		/// </summary>
		/// <param name="scannableTypes"></param>
		private static void RunIInitializationRoutines(IEnumerable<Type> scannableTypes) => scannableTypes.
			// Find all types that can be assigned to IInitializationRoutine
			Where((type) => typeof(IInitializationRoutine).IsAssignableFrom(type)).
			// Create an instance of each
			Select((type) => Activator.CreateInstance(type)).
			// Cast it to IInitializationRoutine
			Cast<IInitializationRoutine>().
			// Call InitializationRoutine on each instance
			ForEach((instance) => instance.InitializationRoutine());

		/// <summary>
		/// Finds all types implementing <see cref="IInitializationRoutine"/>, creates an instance of them and calls
		/// <see cref="IInitializationRoutine.InitializationRoutine"/>
		/// </summary>
		/// <param name="scannableTypes"></param>
		private static void RunIInitializationTypeScans(IEnumerable<Type> scannableTypes) => scannableTypes.
			// Find all types that can be assigned to IInitializationTypeScan
			Where((type) => typeof(IInitializationTypeScan).IsAssignableFrom(type)).
			// Create an instance of each
			Select((type) => Activator.CreateInstance(type)).
			// Cast it to IInitializationTypeScan
			Cast<IInitializationTypeScan>().
			// Call InitializationTypeScan on each instance using types that match any of the given predicates
			ForEach((instance) => instance.InitializationTypeScan(scannableTypes.WhereAny(instance.GetTypeScanPredicates())));

		#endregion

		#region Public static methods

		/// <summary>
		/// Performs all actions that are necessary on app's start-up. It is done that way so that accidental intrusion into private
		/// types (eg. internal or private nested) always has only one possible source - which is this method. It also guarantees that
		/// all private types remain private and invisible outside of their scopes and are not passed to type/assembly scanning methods
		/// by default without their knowledge. Ultimately only classes that implement <see cref="IInitializationTypeScan"/>
		/// determine all types that they wish to have passsed to calls to their respective
		/// <see cref="IInitializationTypeScan.InitializationTypeScan(IEnumerable{Type})"/> methods.
		/// </summary>
		/// <param name="scannableTypes">All types that were declared for ECAT assemblies (that will be used during runtime)</param>
		public static void Run(IEnumerable<Type> scannableTypes)
		{
			RunIInitializationRoutines(scannableTypes);

			RunIInitializationTypeScans(scannableTypes);
		}

		#endregion
	}
}