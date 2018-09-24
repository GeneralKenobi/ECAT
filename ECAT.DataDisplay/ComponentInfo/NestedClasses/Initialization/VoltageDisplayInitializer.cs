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
				// Try to get both terminals
				if (type.TryGetProperty<ITerminal>(attribute.TerminalA, out var terminalA) &&
					type.TryGetProperty<ITerminal>(attribute.TerminalB, out var terminalB))
				{
					// Add a new info section definition to the list
					infoSection = new InfoSectionDefinition(
						new VoltageInfoResolver(type, terminalA, terminalB),
						new VoltageInfoInterpreter(),
						attribute.SectionIndex);

					return true;
				}
				else
				{
					// Log failure when debugging
					LogIncorrectTerminalPropertyNames(type, attribute);

					// Assign null
					infoSection = null;

					// Return failure
					return false;
				}
			}

			#endregion
		}
	}
}