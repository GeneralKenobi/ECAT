using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ECAT.UWP
{
	/// <summary>
	/// Control for sockets on wires
	/// </summary>
    public class WireSocketTC : Control
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public WireSocketTC()
		{
			this.DefaultStyleKey = typeof(WireSocketTC);
			this.Tapped += SocketTapped;			
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Sets the tapped event to handled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketTapped(object sender, TappedRoutedEventArgs e) => e.Handled = true;

		#endregion

		#region Position Dependency Property

		/// <summary>
		/// Position of this TemplatedControl
		/// </summary>
		public IPlanePosition Position
		{
			get => (IPlanePosition)GetValue(PositionProperty);
			set => SetValue(PositionProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="Position"/>
		/// </summary>
		public static readonly DependencyProperty PositionProperty =
			DependencyProperty.Register(nameof(Position), typeof(IPlanePosition),
			typeof(WireSocketTC), new PropertyMetadata(default(IPlanePosition)));

		#endregion

		#region PositionOnWire Dependency Property

		/// <summary>
		/// Indicates the position on the wire (true for ending, false for beginning)
		/// </summary>
		public bool PositionOnWire
		{
			get => (bool)GetValue(PositionOnWireProperty);
			set => SetValue(PositionOnWireProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="PositionOnWire"/>
		/// </summary>
		public static readonly DependencyProperty PositionOnWireProperty =
			DependencyProperty.Register(nameof(PositionOnWire), typeof(bool),
			typeof(WireSocketTC), new PropertyMetadata(default(bool)));

		#endregion
	}
}