using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CSharpEnhanced.Helpers;
using ECAT.Core;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Provides data that should be displayed in the component info section
		/// </summary>
		private class ComponentInfo : IComponentInfo
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public ComponentInfo(IEnumerable<InfoSectionDefinition> infoSections)
			{
				if (infoSections == null)
				{
					throw new ArgumentNullException(nameof(infoSections));
				}

				_Sections = new List<Tuple<InfoSectionDefinition, ComponentInfoSectionHeader>>(infoSections.
					Select((section) => Tuple.Create(section, new ComponentInfoSectionHeader(section.Index, section.Header))));

				// Create the enumerator
				_CurrentSection = _Sections.GetEnumerator();
				// Move it to the first element
				if(_CurrentSection.MoveNext())
				{
					// If it was possible, set the section to be selected
					_CurrentSection.Current.Item2.IsSelected = true;
				}
			}

			/// <summary>
			/// Constructor that takes component that is presented from construction
			/// </summary>
			public ComponentInfo(IEnumerable<InfoSectionDefinition> infoSections, IBaseComponent component) : this(infoSections)
			{
				Update(component);
			}

			#endregion

			#region Events

			/// <summary>
			/// Event fired whenever a property changes its value
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			#endregion

			#region Private properties

			/// <summary>
			/// Contains info sections and headers created for them
			/// </summary>
			private List<Tuple<InfoSectionDefinition, ComponentInfoSectionHeader>> _Sections { get; }

			/// <summary>
			/// Enumerator used to iterate <see cref="_Sections"/>
			/// </summary>
			private IEnumerator<Tuple<InfoSectionDefinition, ComponentInfoSectionHeader>> _CurrentSection { get; }

			#endregion

			#region Public properties

			/// <summary>
			/// Info from the current section
			/// </summary>
			public IEnumerable<string> InterpretedInfo { get; private set; }

			/// <summary>
			/// Headers of all sections
			/// </summary>
			public IEnumerable<IComponentInfoSectionHeader> SectionHeaders => _Sections.Select((x) => x.Item2);

			/// <summary>
			/// The presented info
			/// </summary>
			public ISignalInformation Info { get; private set; }

			/// <summary>
			/// Number of all presented sections 
			/// </summary>
			public int SectionsCount => _Sections.Count;

			#endregion

			#region Public methods

			/// <summary>
			/// Updates the display for <paramref name="component"/>
			/// </summary>
			/// <param name="component"></param>
			public void Update(IBaseComponent component)
			{
				// Get the new info
				var newInfo = _CurrentSection.Current.Item1.GetInfo(component);

				// Assign it to proper properties
				Info = newInfo.Item1;
				InterpretedInfo = newInfo.Item2;				
			}

			/// <summary>
			/// Advances to the next section
			/// </summary>
			public void GoToNextSection()
			{
				// Don't do anything if there are no sections
				if(_Sections.Count == 0)
				{
					return;
				}

				// Deselect the current header
				_CurrentSection.Current.Item2.IsSelected = false;

				// Move to the next section
				_CurrentSection.MoveNextWrapping();

				// Select its header
				_CurrentSection.Current.Item2.IsSelected = true;

				// If there's a focused component
				if(IoC.Resolve<IFocusManager>().FocusedComponent != null)
				{
					// Update self
					Update(IoC.Resolve<IFocusManager>().FocusedComponent);
				}
			}

			#endregion
		}
	}
}