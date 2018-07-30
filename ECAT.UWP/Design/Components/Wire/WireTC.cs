using ECAT.ViewModel;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace ECAT.UWP
{
	/// <summary>
	/// Templated control for a wire
	/// </summary>
	public sealed class WireTC : BorderWithFlyoutMenuBaseTC
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public WireTC() : base("RootGrid")
		{
			this.DefaultStyleKey = typeof(WireTC);
			this.DataContextChanged += OnDataContextChanged;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Command executed when the wire is clicked, invoked from code behind due to the necessity to provide the click position
		/// as the parameter
		/// </summary>
		private ICommand _WireClickedCommand { get; set; }

		#endregion

		#region Private methods

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

		#endregion
	}
}