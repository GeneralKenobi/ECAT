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
		private abstract class InitializerBase<T> : IInitializationTypeScan
			where T : DisplayInfo
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
			/// Method called on each type with each T attribute defined for it that should try to construct an InfoSectionDefinition
			/// and return true on success.
			/// </summary>
			/// <param name="type"></param>
			/// <param name="attribute"></param>
			/// <param name="infoSection"></param>
			/// <returns></returns>
			protected abstract bool TryConstructInfoSection(Type type, T attribute, out InfoSectionDefinition infoSection);

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

				// Filter out the types that don't implement IBaseComponent
				foundTypes = foundTypes.Where((type) => typeof(IBaseComponent).IsAssignableFrom(type));

				// Create a dictionary of infosections keyed by their target types
				var result = new Dictionary<Type, IEnumerable<InfoSectionDefinition>>();

				foreach (var type in foundTypes)
				{
					// Create a list for info sections
					var infoSections = new List<InfoSectionDefinition>();

					// Get all DisplayVoltageInfo attributes (these may be multiple)
					var attributes = Attribute.GetCustomAttributes(type, typeof(T));

					// For each attribute
					foreach (T attribute in attributes)
					{
						// Try to construct info sections
						if (TryConstructInfoSection(type, attribute, out var infoSection))
						{
							// If successful, add it to the list
							infoSections.Add(infoSection);
						}
					}

					// Add the sections to result
					result.Add(type, infoSections);
				}

				// Get the singletion and add keyed info sections to it
				IoC.Resolve<ComponentInfoDisplay>().AddInfoSections(result);
			}

			#endregion
		}
	}
}