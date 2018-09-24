using ECAT.Core;
using System;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Finds all types with <see cref="DisplayPowerInfo"/> attribute and attempts to construct <see cref="InfoSectionDefinition"/>
		/// based on them and add those to <see cref="ComponentInfoProvider"/> singleton resolved from <see cref="IoC"/>
		/// </summary>
		private class PowerDisplayInitializer : InitializerBase<DisplayPowerInfo>
		{
			#region Protected methods

			/// <summary>
			/// Constructs a <see cref="InfoSectionDefinition"/> based on <paramref name="type"/> and <paramref name="attribute"/>
			/// </summary>
			/// <param name="type"></param>
			/// <param name="attribute"></param>
			/// <param name="infoSection"></param>
			/// <returns></returns>
			protected override bool TryConstructInfoSection(Type type, DisplayPowerInfo attribute, out InfoSectionDefinition infoSection)
			{
				infoSection = new InfoSectionDefinition(
					new PowerInfoResolver(type),
					new PowerInfoInterpreter(),
					attribute.SectionIndex);

				// InfoSectionDefinition for power can always be constructed
				return true;
			}

			#endregion
		}
	}
}