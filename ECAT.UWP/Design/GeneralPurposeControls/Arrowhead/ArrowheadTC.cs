using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// A simple control whose purpose is to be an arrowhead
	/// </summary>
	public sealed class ArrowheadTC : Control
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ArrowheadTC()
		{
			this.DefaultStyleKey = typeof(ArrowheadTC);
		}

		#endregion

		#region RotationAngle Dependency Property

		/// <summary>
		/// Angle of rotation of the arrowhead
		/// </summary>
		public double RotationAngle
		{
			get => (double)GetValue(RotationAngleProperty);
			set => SetValue(RotationAngleProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="RotationAngle"/>
		/// </summary>
		public static readonly DependencyProperty RotationAngleProperty =
			DependencyProperty.Register(nameof(RotationAngle), typeof(double),
			typeof(ArrowheadTC), new PropertyMetadata(default(double)));

		#endregion
	}
}