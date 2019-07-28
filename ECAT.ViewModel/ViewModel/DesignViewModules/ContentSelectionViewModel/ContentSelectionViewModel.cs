using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Viewmodel for content selection control in sidemenu
	/// </summary>
	public class ContentSelectionViewModel : BaseViewModel
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ContentSelectionViewModel()
		{
			ShowSchematicCommand = new RelayCommand(ShowSchematic);
			ShowVoltmeterMeasurementCommand = new RelayParametrizedCommand(ShowVoltmeterMeasurement);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Contians declared voltmeters measurements
		/// </summary>
		public ISimulationResultsProvider SimulationResultsProvider { get; } = IoC.Resolve<ISimulationResultsProvider>();

		#endregion

		#region Commands

		/// <summary>
		/// After execution the schematic is shown in the main content section
		/// </summary>
		public ICommand ShowSchematicCommand { get; }

		/// <summary>
		/// After execution the voltage drop corresponding to <see cref="IVoltmeterMeasurement"/> passed as parameter is shown in main content section
		/// </summary>
		public ICommand ShowVoltmeterMeasurementCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="ShowSchematicCommand"/>
		/// </summary>
		private void ShowSchematic()
		{

		}

		/// <summary>
		/// Method for <see cref="ShowVoltmeterReadingCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void ShowVoltmeterMeasurement(object parameter)
		{

		}

		#endregion
	}
}