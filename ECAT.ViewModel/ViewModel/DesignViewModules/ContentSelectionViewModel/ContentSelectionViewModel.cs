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

		/// <summary>
		/// Header to display in this section
		/// </summary>
		public string Header { get; } = "Views";

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
			AppViewModel.Singleton.DesignVM.GraphToShow = null;
			AppViewModel.Singleton.DesignVM.GraphHeader = string.Empty;
		}

		/// <summary>
		/// Method for <see cref="ShowVoltmeterReadingCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void ShowVoltmeterMeasurement(object parameter)
		{
			if(parameter is IVoltmeterMeasurement measurement)
			{
				AppViewModel.Singleton.DesignVM.GraphToShow = IoC.Resolve<ISimulationResultsProvider>().Value.Voltage.Get(measurement.NodeA, measurement.NodeB);
				AppViewModel.Singleton.DesignVM.GraphHeader = measurement.VoltmeterID;
			}
		}

		#endregion
	}
}