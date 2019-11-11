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
	public class SimulationMenuViewModel : BaseViewModel
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SimulationMenuViewModel(ISimulationManager simulationManager, IDesignManager designManager)
		{
			_SimulationManager = simulationManager ?? throw new ArgumentNullException(nameof(simulationManager));
			_DesignManager = designManager ?? throw new ArgumentNullException(nameof(designManager));

			DCBiasCommand = new RelayCommand(() => SimulationMethodWrapper(_SimulationManager.DCBias));
			ACDCFullCycleCommand = new RelayCommand(() => SimulationMethodWrapper(_SimulationManager.ACDCFullCycle));
			FrequencySweepCommand = new RelayCommand(() => SimulationMethodWrapper(_SimulationManager.FrequencySweep));
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Simulation Manager for this app
		/// </summary>
		private ISimulationManager _SimulationManager { get; }

		/// <summary>
		/// Design Manager for this app
		/// </summary>
		private IDesignManager _DesignManager { get; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Header to display in this section
		/// </summary>
		public string Header { get; } = "Run Simulation";

		#endregion

		#region Commands

		/// <summary>
		/// Performs a DC bias simulation of the circuit.
		/// </summary>
		/// <param name="schematic"></param>
		public ICommand DCBiasCommand { get; }

		/// <summary>
		/// Performs a full ACDC simulation
		/// </summary>
		/// <param name="schematic"></param>
		public ICommand ACDCFullCycleCommand { get; }

		/// <summary>
		/// Performs a full ACDC simulation with <see cref="IOpAmp"/> adjustment - every <see cref="IOpAmp"/> is operating in either active or
		/// saturated state so that its output voltage does not exceed its supply voltages.
		/// </summary>
		/// <param name="schematic"></param>
		public ICommand FrequencySweepCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Wrapper invoking <paramref name="simulationFunc"/> if <paramref name="parameter"/> is an <see cref="ISchematic"/>
		/// </summary>
		/// <param name="simulationFunc"></param>
		/// <param name="parameter"></param>
		private void SimulationMethodWrapper(Action<ISchematic> simulationFunc)
		{
			if(_DesignManager.CurrentSchematic != null)
			{
				simulationFunc(_DesignManager.CurrentSchematic);
			}
		}

		#endregion
	}
}