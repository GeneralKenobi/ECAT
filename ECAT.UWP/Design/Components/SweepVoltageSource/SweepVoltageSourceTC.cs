using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for an <see cref="ISweepVoltageSource"/>
	/// </summary>
	public sealed class SweepVoltageSourceTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SweepVoltageSourceTC()
		{
			this.DefaultStyleKey = typeof(SweepVoltageSourceTC);
		}

		#endregion
	}
}