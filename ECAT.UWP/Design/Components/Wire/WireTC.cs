using ECAT.Design;
using ECAT.ViewModel;
using System.Windows.Input;
using UWPEnhanced.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace ECAT.UWP
{
	/// <summary>
	/// Templated control for a wire
	/// </summary>
	public sealed class WireTC : BorderWithFlyoutMenuBaseTC
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public WireTC() : base("RootGrid")
		{
			this.DefaultStyleKey = typeof(WireTC);
			this.DataContextChanged += OnDataContextChanged;

			this.Tapped += OnWireTapped;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Command executed when the wire is clicked, invoked from code behind due to the necessity to provide the click position
		/// as the parameter
		/// </summary>
		private ICommand _WireClickedCommand { get; set; }

		#endregion

		#region Privatem methods

		/// <summary>
		/// Checks if the new DataContext is a <see cref="WireViewModel"/>, if so extracts the
		/// <see cref="WireViewModel.WireClickedCommand"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
		{
			if(args.NewValue is WireViewModel newVM)
			{
				_WireClickedCommand = newVM.WireClickedCommand;
			}
		}

		/// <summary>
		/// Sets the handled flag, extracts click position and executes <see cref="_WireClickedCommand"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnWireTapped(object sender, TappedRoutedEventArgs e)
		{
			e.Handled = true;

			if(e.OriginalSource is FrameworkElement element && element.Name == "WirePolyline")
			{ 
				// Because the wire is considered to be positioned 0,0 relative to the design area (it's position is set using a
				// render transform), the click position relative to the control is the position of the click relative to design area
					var clickPosition = e.GetPosition(this);

				_WireClickedCommand?.Execute(new PlanePosition(clickPosition.X, -clickPosition.Y));
			}
		}

		#endregion
	}
}