using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for an <see cref="ICapacitor"/>
	/// </summary>
	public sealed class CapacitorTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CapacitorTC()
		{
			this.DefaultStyleKey = typeof(CapacitorTC);
		}

		#endregion
	}
}