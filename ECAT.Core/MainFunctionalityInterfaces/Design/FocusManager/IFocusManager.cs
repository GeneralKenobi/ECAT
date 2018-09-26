using System;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class managing focus on components. Component is focused when, for example (it boils down to what UI will
	/// deem as focus action but it can be generalized to), pointer is over the component or the component was tapped.
	/// </summary>
	public interface IFocusManager
    {
		#region Events

		/// <summary>
		/// Event fired when focus changes - after the old component lost focus and the new component got focus.
		/// </summary>
		event EventHandler<FocusedComponentChangedEventArgs> FocusedComponentChanged;

		#endregion

		#region Properties

		/// <summary>
		/// The component that is currently focused or null if no component has focus
		/// </summary>
		IBaseComponent FocusedComponent { get; }

		#endregion
	}
}