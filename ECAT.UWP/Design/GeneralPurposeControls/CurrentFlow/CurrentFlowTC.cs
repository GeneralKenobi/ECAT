using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control visualizing a current flow across a 2-terminal element
	/// </summary>
	public sealed class CurrentFlowTC : Control
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentFlowTC()
        {
            this.DefaultStyleKey = typeof(CurrentFlowTC);
        }

		#endregion

		#region Current Dependency Property

		/// <summary>
		/// The value of the current, flowing from left to right
		/// </summary>
		public Complex Current
		{
			get => (Complex)GetValue(CurrentProperty);
			set => SetValue(CurrentProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="Current"/>
		/// </summary>
		public static readonly DependencyProperty CurrentProperty =
			DependencyProperty.Register(nameof(Current), typeof(Complex),
			typeof(CurrentFlowTC), new PropertyMetadata(default(Complex)));

		#endregion
	}
}