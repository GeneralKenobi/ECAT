using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ECAT.UWP
{
	/// <summary>
	/// Control which visualizes a voltage drop from one point to the other using an arrow. For negative voltages the arrow is
	/// reversed instead of displaying a minus sign in front of them
	/// </summary>
	public sealed class VoltageDropTC : Control, INotifyPropertyChanged
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageDropTC()
		{
			this.DefaultStyleKey = typeof(VoltageDropTC);
			this.SizeChanged += SizeChangedCallback;
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// Height of the row with value display
		/// </summary>
		public double ValueRowHeight { get; } = 25;

		/// <summary>
		/// Describes the geometry to assgin to Path's Data property (a curve constructed with use of a Bezier segment)
		/// </summary>
		public Geometry PathGeometry => new PathGeometry()
		{
			Figures = new PathFigureCollection()
			{
				new PathFigure()
				{
					// Start at the bottom
					StartPoint = new Point(0, ActualHeight - ValueRowHeight),
					Segments =new PathSegmentCollection()
					{
						new BezierSegment()
						{
							// Starting point
							Point1 = new Point(0, ActualHeight - ValueRowHeight),
							// Middle at the top (control point, actual curve will be drawn more or less in the middle)
							Point2 = new Point(ActualWidth / 2, -(ActualHeight - ValueRowHeight)),
							// Finish at the bottom again
							Point3 = new Point(ActualWidth, ActualHeight - ValueRowHeight),
						}
					},					
				}
			}
		};

		#endregion

		#region Voltage Dependency Property

		/// <summary>
		/// The voltage presented by this voltage drop arrow
		/// </summary>
		public double Voltage
		{
			get => (double)GetValue(VoltageProperty);
			set => SetValue(VoltageProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="Voltage"/>
		/// </summary>
		public static readonly DependencyProperty VoltageProperty =
			DependencyProperty.Register(nameof(Voltage), typeof(double),
			typeof(VoltageDropTC), new PropertyMetadata(default(double)));

		#endregion

		#region ArrowRotationAngle Dependency Property

		/// <summary>
		/// Angle of rotation for arrows (in degrees, describe for the left arrow, right arrow will be computed accordingly)
		/// </summary>
		public double ArrowRotationAngle
		{
			get => (double)GetValue(ArrowRotationAngleProperty);
			set => SetValue(ArrowRotationAngleProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="ArrowRotationAngle"/>
		/// </summary>
		public static readonly DependencyProperty ArrowRotationAngleProperty =
			DependencyProperty.Register(nameof(ArrowRotationAngle), typeof(double),
			typeof(VoltageDropTC), new PropertyMetadata(default(double)));

		#endregion

		#region Private methods

		/// <summary>
		/// Callback for when size changes (recompute path data)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SizeChangedCallback(object sender, SizeChangedEventArgs e) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathGeometry)));

		#endregion
	}
}