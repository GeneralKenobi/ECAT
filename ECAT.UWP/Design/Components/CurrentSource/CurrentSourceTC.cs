using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Templated control for an <see cref="ICurrentSource"/>
	/// </summary>
	public sealed class CurrentSourceTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentSourceTC()
		{
			this.DefaultStyleKey = typeof(CurrentSourceTC);
		}

		#endregion
	}
}