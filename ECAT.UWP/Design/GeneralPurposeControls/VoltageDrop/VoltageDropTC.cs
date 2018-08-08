using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control which visualizes a voltage drop from one point to the other using an arrow. For negative voltages the arrow is
	/// reversed instead of displaying a minus sign in front of them
	/// </summary>
	public sealed class VoltageDropTC : Control
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageDropTC()
		{
			this.DefaultStyleKey = typeof(VoltageDropTC);
		}

		#endregion
	}
}