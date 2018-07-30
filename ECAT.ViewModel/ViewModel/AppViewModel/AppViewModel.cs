using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for the app in general (eg. for the main page)
	/// </summary>
	public class AppViewModel : BaseViewModel
    {
		#region Singleton

		/// <summary>
		/// The singleton ViewModel for the whole app
		/// </summary>
		public static AppViewModel Singleton { get; } = new AppViewModel();

		#endregion

		#region Public properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		public double RoundTo { get; } = IoC.Resolve<IPlanePositionFactory>().RoundTo;
		
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
	}
}