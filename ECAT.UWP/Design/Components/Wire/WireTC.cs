using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPEnhanced.Xaml;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

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
		}

		#endregion
	}
}