using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for an <see cref="IVoltageSource"/>
	/// </summary>
	public sealed class VoltageSourceTC : Control
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageSourceTC()
		{
			this.DefaultStyleKey = typeof(VoltageSourceTC);
		}

		#endregion
	}
}