using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for an <see cref="IACVoltageSource"/>
	/// </summary>
	public sealed class ACVoltageSourceTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ACVoltageSourceTC()
		{
			this.DefaultStyleKey = typeof(ACVoltageSourceTC);
		}

		#endregion
	}
}