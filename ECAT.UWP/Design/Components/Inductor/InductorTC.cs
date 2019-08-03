using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for an <see cref="IInductor"/>
	/// </summary>
	public sealed class InductorTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public InductorTC()
		{
			this.DefaultStyleKey = typeof(InductorTC);
		}

		#endregion
	}
}