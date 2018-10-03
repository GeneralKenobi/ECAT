using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.ICommands;
using ECAT.Core;
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
		/// Default constructor
		/// </summary>
		public AppViewModel()
		{
			ProcessKeyPressCommand = new RelayParametrizedCommand(ProcessKeyPress);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		public double RoundTo { get; } = IoC.Resolve<IDefaultValues>().RoundToCoordinates;
		
		/// <summary>
		/// ViewModel related with circuit design functionalities
		/// </summary>
		public DesignViewModel DesignVM { get; } = new DesignViewModel();

		/// <summary>
		/// ViewModel related with circuit simulation functionalities
		/// </summary>
		public SimulationViewModel SimulationVM { get; } = new SimulationViewModel();

		/// <summary>
		/// Manages shortcuts and processes key presses
		/// </summary>
		public ShortcutManager ShortcutManager { get; } = new ShortcutManager();

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

				if(DesignVM.DesignManager.PlacingWire)
				{
					return AppState.PlacingWire;
				}

				return AppState.Idle;
			}
		}

		#endregion

		#region Commands

		/// <summary>
		/// Processes key presses - if the parameter is a <see cref="KeyArgument"/> determines whether it corresponds to a shortcut
		/// </summary>
		public ICommand ProcessKeyPressCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="ProcessKeyPressCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void ProcessKeyPress(object parameter)
		{
			if(parameter is KeyArgument key)
			{
				ShortcutManager.ProcessKeyCombination(key);
			}
		}

		#endregion

		#region Singleton

		/// <summary>
		/// The singleton ViewModel for the whole app
		/// </summary>
		public static AppViewModel Singleton { get; } = new AppViewModel();

		#endregion
	}
}