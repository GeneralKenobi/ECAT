using PropertyChanged;
using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ECAT.UWP
{
	public sealed class VoltageDropTC : Control, INotifyPropertyChanged
	{
		public VoltageDropTC()
		{
			this.DefaultStyleKey = typeof(VoltageDropTC);
			this.SizeChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathGeometry)));
		}


		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

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

		public void UpdatePathData()
		{
			//string dataString = 
			//
			//var path = this.GetTemplateChild("PathControl") as Path;
			//
			//path.SetValue(Path.DataProperty, )

			var path = new BezierSegment();
			path.Point1 = new Point(0, 0);
			path.Point2 = new Point(50, 50);
			path.Point3 = new Point(100, 0);			
		}

		public Geometry PathGeometry => new PathGeometry()
		{
			Figures = new PathFigureCollection()
			{
				new PathFigure()
				{
					StartPoint = new Point(0,ActualHeight),
					Segments =new PathSegmentCollection()
					{
						new BezierSegment()
						{
							Point1 = new Point(0, ActualHeight),
							Point2 = new Point(ActualWidth / 2, 0),
							Point3 = new Point(ActualWidth, ActualHeight),
						}
					}
				}
			}
		};

	}
}