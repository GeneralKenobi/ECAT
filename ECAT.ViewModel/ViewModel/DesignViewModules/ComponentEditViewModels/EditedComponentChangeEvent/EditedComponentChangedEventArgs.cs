using System;

namespace ECAT.ViewModel
{
	/// <summary>
	/// EventArgs for events that signal changes of edited components
	/// </summary>
	public class EditedComponentChangedEventArgs : EventArgs
    {
		#region Constructor

		/// <summary>
		/// Constructor with a parameter
		/// </summary>
		/// <param name="change"></param>
		public EditedComponentChangedEventArgs(EditedComponentChanged change)
		{
			Change = change;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Type of change that occured
		/// </summary>
		public EditedComponentChanged Change { get; }

		#endregion
	}
}