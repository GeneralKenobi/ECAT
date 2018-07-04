using ECAT.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        }

		#endregion

		#region PartialNode Dependency Property

		/// <summary>
		/// 
		/// </summary>
		public IPartialNode PartialNode
		{
			get => (IPartialNode)GetValue(PartialNodeProperty);
			set => SetValue(PartialNodeProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="PartialNode"/>
		/// </summary>
		public static readonly DependencyProperty PartialNodeProperty =
			DependencyProperty.Register(nameof(PartialNode), typeof(IPartialNode),
			typeof(SocketTC), new PropertyMetadata(default(IPartialNode)));

		#endregion
	}
}