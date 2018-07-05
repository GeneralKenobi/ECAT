using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for the app in general (eg. for the main page)
	/// </summary>
    public class AppViewModel : BaseViewModel
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		private AppViewModel()
		{
			DesignAreaClickedCommand = new RelayParametrizedCommand(DesignAreaClicked);
		}

		#endregion

		#region Singleton

		/// <summary>
		/// The singleton ViewModel for the whole app
		/// </summary>
		public static AppViewModel Singleton { get; } = new AppViewModel();

		#endregion

		#region Public properties

		/// <summary>
		/// ViewModel related with circuit design functionalities
		/// </summary>
		public DesignViewModel DesignVM { get; } = new DesignViewModel();

		/// <summary>
		/// ViewModel related with circuit simulation functionalities
		/// </summary>
		public SimulationViewModel SimulationVM { get; } = new SimulationViewModel();

		/// <summary>
		/// Returns the current state of the wire
		/// </summary>
		public AppState State
		{
			get
			{
				if(DesignVM.AddingComponents)
				{
					return AppState.AddingComponents;
				}

				if(DesignVM.PlacingWire)
				{
					return AppState.PlacingWire;
				}

				return AppState.Idle;
			}
		}

		#endregion

		#region Commands

		/// <summary>
		/// Command to invoke when the user clicks (presses, etc) the design area on the screen. The parameter should be the coordinate
		/// of the click given as a <see cref="PlanePosition"/>
		/// </summary>
		public ICommand DesignAreaClickedCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="DesignAreaClickedCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void DesignAreaClicked(object parameter)
		{
			if(parameter is IPlanePosition position)
			{
				// Decide what to do with the input
				switch(State)
				{
					case AppState.AddingComponents:
						{
							DesignVM.AddComponent(position);
						} break;

					case AppState.PlacingWire:
						{
							DesignVM.AddPointToPlacedWire(position);
						} break;
				}
			}
		}

		#endregion
	}
}
