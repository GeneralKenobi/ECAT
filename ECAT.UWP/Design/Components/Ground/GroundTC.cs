using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Templated control for an <see cref="IGround"/>
	/// </summary>
	public sealed class GroundTC : Control
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public GroundTC()
		{
			this.DefaultStyleKey = typeof(GroundTC);
		}

		#endregion
	}
}