using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ECAT.UWP
{
	public sealed class ArrowheadTC : Control
	{
		#region Constructor

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