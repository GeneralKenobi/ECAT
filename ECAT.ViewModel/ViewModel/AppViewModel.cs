using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for the app in general (eg. for the main page)
	/// </summary>
    public class AppViewModel : BaseViewModel
    {
		public DesignViewModel DesignVM { get; set; } = new DesignViewModel();
    }
}
