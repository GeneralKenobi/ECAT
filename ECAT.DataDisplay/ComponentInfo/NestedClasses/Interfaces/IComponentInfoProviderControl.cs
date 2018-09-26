using System;
using System.Collections.Generic;

namespace ECAT.DataDisplay
{
	partial class ComponentInfoProvider
	{
		/// <summary>
		/// Interface used to control behaviour of <see cref="Core.IComponentInfoProvider"/>
		/// </summary>
		private interface IComponentInfoProviderControl
		{
			#region Methods

			/// Adds new info sections to. Each type has to implement <see cref="IBaseComponent"/>
			/// (otherwise an exception is thrown).
			void AddInfoSections(IEnumerable<KeyValuePair<Type, IEnumerable<InfoSectionDefinition>>> sections);

			#endregion
		}
	}
}