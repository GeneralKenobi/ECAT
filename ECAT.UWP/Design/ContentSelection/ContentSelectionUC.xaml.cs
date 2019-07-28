using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        }

		#endregion
	}
}
