using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Manages component info display
	/// </summary>
	[RegisterAsInstance(typeof(ComponentInfoDisplay))]
	public partial class ComponentInfoDisplay
    {
		#region Private properties

		/// <summary>
		/// Info sections defined for types that want to display them. Each type implements <see cref="IBaseComponent"/>.
		/// </summary>
		private Dictionary<Type, SortedSet<InfoSectionDefinition>> _DisplaySettings { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Adds new info sections to <see cref="_DisplaySettings"/>. Each type has to implement <see cref="IBaseComponent"/>
		/// (otherwise an exception is thrown).
		/// </summary>
		/// <param name="sections"></param>
		private void AddInfoSections(IEnumerable<KeyValuePair<Type, IEnumerable<InfoSectionDefinition>>> sections)
		{			
			foreach(var entry in sections)
			{
				// Check if the type implements IBaseComponent
				if(typeof(IBaseComponent).IsAssignableFrom(entry.Key))
				{
					throw new Exception(entry.Key.FullName + " does not implement " + nameof(IBaseComponent));
				}

				// If there is no entry corresponding to that type
				if(!_DisplaySettings.ContainsKey(entry.Key))
				{
					// Add a new one with sorted set for section definitions collection
					_DisplaySettings.Add(entry.Key, new SortedSet<InfoSectionDefinition>(
						// Compare the indexes
						new CustomComparer<InfoSectionDefinition>((x,y) => x.Index.CompareTo(y.Index))));
				}

				// Now that there for sure is an entry for the type add each info section to the collection
				entry.Value.ForEach((section) => _DisplaySettings[entry.Key].Add(section));
			}
		}

		#endregion
	}
}