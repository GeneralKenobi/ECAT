using ECAT.Core;
using ECAT.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace ECAT.UWP
{
	/// <summary>
	/// UserControl allowing to select content displayed on main section of the page (schematic, measurement, etc.)
	/// </summary>
    public sealed partial class ContentSelectionUC : UserControl
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ContentSelectionUC()
        {
            this.InitializeComponent();
			this.DataContextChanged += OnDataContextChanged;
        }


		#endregion

		#region Private properties

		/// <summary>
		/// Command used to Voltmeter Measurement in main component section
		/// </summary>
		private ContentSelectionViewModel ViewModel { get; set; }

		#endregion

		#region Private Methods

		/// <summary>
		/// Tries to cast new DataContext to expected type and assigns it to <see cref="ViewModel"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) => ViewModel = args.NewValue as ContentSelectionViewModel;

		/// <summary>
		/// Invokes command that changes content currently displayed in main section to the clicked item's measurement 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListViewItemClick(object sender, ItemClickEventArgs e)
		{
			if(e.ClickedItem is IVoltmeterMeasurement && ViewModel != null)
			{
				ViewModel.ShowVoltmeterMeasurementCommand.Execute(e.ClickedItem);
			}
		}

		#endregion
	}
}
