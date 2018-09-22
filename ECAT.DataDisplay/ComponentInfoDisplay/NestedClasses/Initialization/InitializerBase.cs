using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Base class for initializers of <see cref="InfoSectionDefinition"/>s for specific signals (voltage, current, etc.).
		/// From the found types chooses only those that implement <see cref="IBaseComponent"/> (and when in debug writes to
		/// debug information about each that that was marked but doesn't implement the interface) to
		/// <see cref="InitializationTypeScanRoutine(IEnumerable{Type})"/>.
		/// </summary>
		internal abstract class InitializerBase : IInitializationTypeScan
		{
			#region Private methods

			/// <summary>
			/// Finds all types that were marked with display info attribute but do not implement <see cref="IBaseComponent"/> and
			/// informs about each of them by calling <see cref="Debug.WriteLine(object)"/>
			/// </summary>
			/// <param name="types"></param>
			[Conditional("DEBUG")]
			private void LogTypesNotImplementingIBaseComponent(IEnumerable<Type> types) => types.
				// Get all types that can't be assigned to IBaseComponent
				Where((type) => !typeof(IBaseComponent).IsAssignableFrom(typeof(IBaseComponent))).
				// And write a line to Debug for each of them
				ForEach((type) => Debug.WriteLine(
					$"Type ({type.FullName}) was marked with display info attribute but it does not implement to {nameof(IBaseComponent)}"));

			#endregion

			#region Protected methods

			/// <summary>
			/// Method called by <see cref="InitializationTypeScan(IEnumerable{Type})"/> with types determined by
			/// <see cref="GetTypeScanPredicates"/> and checked to implement <see cref="IBaseComponent"/>
			/// </summary>
			/// <param name="types"></param>
			protected abstract void InitializationTypeScanRoutine(IEnumerable<Type> types);

			#endregion

			#region Public methods

			/// <summary>
			/// Returns predicates that match the given initializer's needs.
			/// </summary>
			/// <returns></returns>
			public abstract IEnumerable<Predicate<Type>> GetTypeScanPredicates();			

			/// <summary>
			/// Called on start-up with all types determined by <see cref="GetTypeScanPredicates"/>, additionally checks if all types
			/// implement <see cref="IBaseComponent"/> (and if any of them don't, they're removed from sequence).
			/// </summary>
			/// <param name="foundTypes"></param>
			public void InitializationTypeScan(IEnumerable<Type> foundTypes)
			{
				// In Debug notify about all types that were marked with display info parameter but do not implement IBaseComponent
				LogTypesNotImplementingIBaseComponent(foundTypes);

				// Call the protected method with all types that can be assigned to IBaseComponent
				InitializationTypeScanRoutine(foundTypes.Where((type) => typeof(IBaseComponent).IsAssignableFrom(type)));
			}

			#endregion
		}
	}
}