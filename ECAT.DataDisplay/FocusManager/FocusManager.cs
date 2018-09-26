using ECAT.Core;
using System;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Manages giving and taking focus from <see cref="IBaseComponent"/>s
	/// </summary>
	[RegisterAsInstance(typeof(IFocusManager), typeof(IFocusManagerControl))]
	internal class FocusManager : IFocusManager, IFocusManagerControl
    {
		#region Events

		/// <summary>
		/// Event fired when focus changes - after the old component lost focus and before the new component got focus.
		/// </summary>
		public event EventHandler<FocusedComponentChangedEventArgs> FocusedComponentChanged;

		#endregion

		#region Properties

		/// <summary>
		/// The component that is currently focused or null if no component has focus
		/// </summary>
		public IBaseComponent FocusedComponent { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Sets focus to <paramref name="component"/>
		/// </summary>
		/// <param name="component"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void SetFocus(IBaseComponent component)
		{
			// Remember the old component
			var old = FocusedComponent;

			// Try to set focus to a new component
			FocusedComponent = component ?? throw new ArgumentNullException(nameof(component));

			// Invoke the event
			FocusedComponentChanged.Invoke(this, new FocusedComponentChangedEventArgs(old, FocusedComponent));
		}

		/// <summary>
		/// Loses focus of <paramref name="component"/> (<paramref name="component"/> has to match the currently focused component,
		/// otherwise focus won't be lost).
		/// </summary>
		/// <param name="component"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void LoseFocus(IBaseComponent component)
		{
			// Check if argument is not null
			if(component == null)
			{
				throw new ArgumentNullException(nameof(component));
			}

			// If the focused component tries to lose focus
			if (FocusedComponent == component)
			{
				// Reset the property
				FocusedComponent = null;

				// And invoke the event
				FocusedComponentChanged.Invoke(this, new FocusedComponentChangedEventArgs(component, null));
			}
		}

		#endregion
	}
}