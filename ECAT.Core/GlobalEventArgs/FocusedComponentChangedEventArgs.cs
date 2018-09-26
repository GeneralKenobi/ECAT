using System;

namespace ECAT.Core
{
	/// <summary>
	/// EventArgs for events signaling that focused component changed
	/// </summary>
	public class FocusedComponentChangedEventArgs : EventArgs
    {
		#region Constructors

		/// <summary>
		/// Default constructor, requires parameters
		/// </summary>
		/// <param name="lostFocus"></param>
		/// <param name="gotFocus"></param>
		public FocusedComponentChangedEventArgs(IBaseComponent lostFocus, IBaseComponent gotFocus)
		{
			LostFocus = lostFocus;
			GotFocus = gotFocus;
		}
		
		#endregion

		#region Public properties

		/// <summary>
		/// Component that lost focus or null if no component was focused
		/// </summary>
		public IBaseComponent LostFocus { get; }

		/// <summary>
		/// Component that got focused or null if no component is focused after the event
		/// </summary>
		public IBaseComponent GotFocus { get; }

		#endregion
	}
}