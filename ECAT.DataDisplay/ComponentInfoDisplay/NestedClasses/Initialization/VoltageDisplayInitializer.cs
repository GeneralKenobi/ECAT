using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Finds all types with <see cref="DisplayVoltageInfo"/> attribute and attempts to construct <see cref="InfoSectionDefinition"/>
		/// based on them and add those to <see cref="ComponentInfoDisplay"/> singleton resoled from <see cref="IoC"/>
		/// </summary>
		internal class VoltageDisplayInitializer : InitializerBase
		{
			#region Private methods

			/// <summary>
			/// Writes to debug information about failure to retrieve both terminals from <paramref name="type"/>
			/// </summary>
			/// <param name="type"></param>
			/// <param name="attribute"></param>
			[Conditional("Debug")]
			private void LogIncorrectTerminalPropertyNames(Type type, DisplayVoltageInfo attribute) =>
				Debug.WriteLine($"Couldn't find all properties of type {typeof(ITerminal).FullName} given by names " +
					$"{attribute.TerminalA} and {attribute.TerminalB} on type {type.FullName}");

			#endregion

			#region Protected methods

			/// <summary>
			/// Creates an info section based on each type and its <see cref="DisplayVoltageInfo"/> attributes
			/// </summary>
			/// <param name="types"></param>
			protected override void InitializationTypeScanRoutine(IEnumerable<Type> types)
			{
				// Create a dictionary of infosections keyed by their target types
				var result = new Dictionary<Type, IEnumerable<InfoSectionDefinition>>();

				foreach (var type in types)
				{
					// Create a list for info sections
					var infoSections = new List<InfoSectionDefinition>();

					// Get all DisplayVoltageInfo attributes (these may be multiple)
					var attributes = Attribute.GetCustomAttributes(type, typeof(DisplayVoltageInfo));

					// For each attribute
					foreach (DisplayVoltageInfo attribute in attributes)
					{
						// Try to get both terminals
						if(type.TryGetProperty<ITerminal>(attribute.TerminalA, out var terminalA) &&
							type.TryGetProperty<ITerminal>(attribute.TerminalB, out var terminalB))
						{
							// Add a new info section definition to the list
							infoSections.Add(new InfoSectionDefinition(new VoltageInfoResolver(), null, attribute.SectionIndex));
						}
						else
						{
							// Log failure when debugging
							LogIncorrectTerminalPropertyNames(type, attribute);
						}
					}

					// Add the sections to result
					result.Add(type, infoSections);
				}

				// Get the singletion and add keyed info sections to it
				IoC.Resolve<ComponentInfoDisplay>().AddInfoSections(result);
			}

			#endregion

			#region Public methods

			/// <summary>
			/// Predicates that check whether types have attributes related to specific information display
			/// </summary>
			/// <returns></returns>
			public override IEnumerable<Predicate<Type>> GetTypeScanPredicates() => new Predicate<Type>[]
			{
				// Voltage drop information attribute
				(type) => Attribute.IsDefined(type, typeof(DisplayVoltageInfo)),
			};

			#endregion
		}
	}
}