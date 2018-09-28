using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Diagnostics;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Finds all types with <see cref="DisplayVoltageInfo"/> attribute and attempts to construct <see cref="InfoSectionDefinition"/>
		/// based on them and add those to <see cref="ComponentInfoProvider"/> singleton resolved from <see cref="IoC"/>
		/// </summary>
		private class VoltageDisplayInitializer : InitializerBase<DisplayVoltageInfo>
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
			/// Tries to construct InfoSectionDefinition for <paramref name="type"/> and <paramref name="attribute"/>.
			/// </summary>
			/// <param name="type"></param>
			/// <param name="attribute"></param>
			/// <param name="infoSection"></param>
			/// <returns></returns>
			protected override bool TryConstructInfoSection(Type type, DisplayVoltageInfo attribute, out InfoSectionDefinition infoSection)
			{
				// Try to get terminalB
				if (type.TryGetProperty<ITerminal>(attribute.TerminalB, out var terminalB))
				{
					// If successful, create a variable for a resolver
					VoltageInfoResolver resolver = null;

					// If TerminalA is an empty string
					if (attribute.TerminalA == string.Empty)
					{
						// Create a resolver from ground to terminal B
						resolver = new VoltageInfoResolver(type, terminalB);
					}
					// Otherwise try to get terminalA
					else if (type.TryGetProperty<ITerminal>(attribute.TerminalA, out var terminalA))
					{
						// If successful create a resolver from terminalA to terminalB
						resolver = new VoltageInfoResolver(type, terminalA, terminalB);
					}

					// If resolver was successfully created
					if(resolver != null)
					{
						// Create a new info section definition
						infoSection = new InfoSectionDefinition(
							resolver,
							new VoltageInfoInterpreter(),
							attribute.SectionIndex,
							attribute.Header);

						// And return success
						return true;
					}
				}
				
				// If the method didn't return by now it means that it failed

				// Log failure when debugging
				LogIncorrectTerminalPropertyNames(type, attribute);

				// Assign null
				infoSection = null;

				// Return failure
				return false;				
			}

			#endregion
		}
	}
}