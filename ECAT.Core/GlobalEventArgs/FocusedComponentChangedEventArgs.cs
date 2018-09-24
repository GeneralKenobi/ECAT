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
		/// <param name="gettingFocus"></param>
		public FocusedComponentChangedEventArgs(IBaseComponent lostFocus, IBaseComponent gettingFocus)
		{
			LostFocus = lostFocus;
			GettingFocus = gettingFocus;
		}
		
		#endregion

		#region Public properties

		/// <summary>
		/// Component that is losing focus or null if no component was focused
		/// </summary>
		public IBaseComponent LostFocus { get; }

		/// <summary>
		/// Component that will get focus or null if no component will be focused
		/// </summary>
		public IBaseComponent GettingFocus { get; }

		#endregion
	}
}