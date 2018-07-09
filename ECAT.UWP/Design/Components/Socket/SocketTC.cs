using ECAT.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ECAT.UWP
{
	/// <summary>
	/// Templated control for sockets
	/// </summary>
	public sealed class SocketTC : Control
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SocketTC()
        {
			this.DefaultStyleKey = typeof(SocketTC);		
			this.Tapped += SocketTapped;
        }

		#endregion

		#region Private methods

		/// <summary>
		/// Sets the handled property
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketTapped(object sender, TappedRoutedEventArgs e) => e.Handled = true;

		#endregion

		#region PartialNode Dependency Property

		/// <summary>
		/// Position assigned to this <see cref="SocketTC"/>
		/// </summary>
		public IPlanePosition Position
		{
			get => (IPlanePosition)GetValue(PartialNodeProperty);
			set => SetValue(PartialNodeProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="Position"/>
		/// </summary>
		public static readonly DependencyProperty PartialNodeProperty =
			DependencyProperty.Register(nameof(Position), typeof(IPlanePosition),
			typeof(SocketTC), new PropertyMetadata(default(IPlanePosition)));

		#endregion
	}
}