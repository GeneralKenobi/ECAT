using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for an <see cref="IVoltmeterTC"/>
	/// </summary>
	public sealed class VoltmeterTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltmeterTC()
		{
			this.DefaultStyleKey = typeof(VoltmeterTC);
		}

		#endregion
	}
}