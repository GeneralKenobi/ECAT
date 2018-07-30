using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for <see cref="IResistor"/>
	/// </summary>
	public sealed class ResistorTC : Control
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ResistorTC()
        {
            this.DefaultStyleKey = typeof(ResistorTC);
        }

		#endregion
	}
}