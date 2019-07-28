using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Viewmodel for content selection control in sidemenu
	/// </summary>
	public class ContentSelectionViewModel : BaseViewModel
	{
		#region Public Properties

		/// <summary>
		/// Contians declared voltmeters measurements
		/// </summary>
		public ISimulationResultsProvider SimulationResultsProvider { get; } = IoC.Resolve<ISimulationResultsProvider>();

		#endregion
	}
}