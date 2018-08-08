using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control visualizing a current flow across a 2-terminal element
	/// </summary>
	public sealed class CurrentFlowTC : Control
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentFlowTC()
        {
            this.DefaultStyleKey = typeof(CurrentFlowTC);
        }

		#endregion
	}
}