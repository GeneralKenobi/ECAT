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
		/// <summary>
		/// ViewModel related with circuit design functionalities
		/// </summary>
		public DesignViewModel DesignVM { get; } = new DesignViewModel();

		/// <summary>
		/// ViewModel related with circuit simulation functionalities
		/// </summary>
		public SimulationViewModel SimulationVM { get; } = new SimulationViewModel();




    }
}
