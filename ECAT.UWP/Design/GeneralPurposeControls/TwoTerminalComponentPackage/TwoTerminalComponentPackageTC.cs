using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ECAT.UWP
{
	/// <summary>
	/// Control providing basic sub-controls to a two terminal component's control. Voltage drop across the component is displayed
	/// at the top as an arrow, current flow (indicated by an arrow) is displayed on the component's wire (which is center by default),
	/// components value is presented below the component
	/// </summary>
	public sealed class TwoTerminalComponentPackageTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public TwoTerminalComponentPackageTC()
		{
			this.DefaultStyleKey = typeof(TwoTerminalComponentPackageTC);
		}

		#endregion
	}
}