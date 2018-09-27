using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Helpers;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Manages component info display
	/// </summary>
	[RegisterAsInstance(typeof(IComponentInfoProvider), typeof(IComponentInfoProviderControl))]
	public partial class ComponentInfoProvider : IComponentInfoProvider, ComponentInfoProvider.IComponentInfoProviderControl
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public ComponentInfoProvider()
		{
			IoC.Resolve<IFocusManager>().FocusedComponentChanged += FocusedComponentChanged;
			IoC.Resolve<ISimulationManager>().SimulationCompleted += SimulationCompleted;
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
		/// Info sections defined for types that want to display them. Each type implements <see cref="IBaseComponent"/>.
		/// </summary>
		private Dictionary<Type, SortedSet<InfoSectionDefinition>> _DisplaySettings { get; } =
			new Dictionary<Type, SortedSet<InfoSectionDefinition>>();

		/// <summary>
		/// Backing store for <see cref="Value"/>
		/// </summary>
		private ComponentInfo _Value { get; set; }

		#endregion

		#region Public properties

		/// <summary>
		/// Currently presented info
		/// </summary>
		public IComponentInfo Value => _Value;

		/// <summary>
		/// True if the current info doesn't provide any value and can be not displayed
		/// </summary>
		public bool CanBeHidden => _Value == null || _Value.SectionsCount == 0;

		#endregion

		#region Private methods

		/// <summary>
		/// Callback for when simulation finishes, if there is a focused element updates <see cref="_Value"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SimulationCompleted(object sender, SimulationCompletedEventArgs e)
		{
			var focused = IoC.Resolve<IFocusManager>().FocusedComponent;

			// If the info is not null and there is a focused element, update the info
			if (_Value != null && focused != null)
			{
				_Value.Update(focused);
			}
		}

		/// <summary>
		/// Callback for when focused component changes; removes, updates or creates a new <see cref="_Value"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FocusedComponentChanged(object sender, FocusedComponentChangedEventArgs e)
		{
			// If the focus did not change (shouldn't happen but it's better to cover this case)
			if (e.LostFocus == e.GotFocus)
			{
				// Don't do anything
				return;
			}

			// If both the old and new component aren't null, they have the same type and _Info is already constructed
			if (e.LostFocus != null && e.GotFocus != null && e.LostFocus.GetType() == e.GotFocus.GetType() && _Value != null)
			{
				// Update it
				_Value.Update(e.GotFocus);

				// And return it
				return;
			}

			// If we got here it means that the _Info will either be removed, changed or constructed so, no matter which case it is,
			// reset _Info
			_Value = null;

			// If an element gets focus and it requests info display
			if (e.GotFocus != null && _DisplaySettings.TryGetValue(e.GotFocus.GetType(), out var infoSections))
			{
				// Construct a ComponentInfo for it
				_Value = new ComponentInfo(infoSections);
			}
		}

		/// <summary>
		/// Adds new info sections to <see cref="_DisplaySettings"/>. Each type has to implement <see cref="IBaseComponent"/>
		/// (otherwise an exception is thrown).
		/// </summary>
		/// <param name="sections"></param>
		void IComponentInfoProviderControl.AddInfoSections(IEnumerable<KeyValuePair<Type, IEnumerable<InfoSectionDefinition>>> sections)
		{			
			foreach(var entry in sections)
			{
				// Check if the type implements IBaseComponent
				if(!typeof(IBaseComponent).IsAssignableFrom(entry.Key))
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

		#region Public methods

		/// <summary>
		/// Moves the info to next section
		/// </summary>
		public void GoToNextSection()
		{
			if(_Value != null)
			{
				_Value.GoToNextSection();
			}
		}

		#endregion
	}
}