using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for components in circuit design
	/// </summary>
	public class ComponentViewModel : BaseViewModel
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComponentViewModel(IBaseComponent component)
		{
			Component = component ?? throw new ArgumentNullException(nameof(component) + " cannot be null");

			// Subscribe to simualtion completed event
			IoC.Resolve<ISimulationManager>().SimulationCompleted += SimulationCompletedCallback;
			
			RotateLeftCommand = new RelayCommand(RotateLeft);
			RotateRightCommand = new RelayCommand(RotateRight);
			RemoveComponentCommand = new RelayCommand(RemoveComponent);
			SocketClickedCommand = new RelayParametrizedCommand(SocketClicked);
			EngageFocusCommand = new RelayCommand(EngageFocus);
			DisengageFocusCommand = new RelayCommand(DisengageFocus);
			ReverseVoltageDropsCommand = new RelayCommand(ReverseVoltageDrops);
			ProceedToAnotherInfoSectionCommand = new RelayCommand(ProceedToAnotherInfoSection);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Length of hitbox for sockets
		/// </summary>
		public double HitboxLength => AppViewModel.Singleton.RoundTo;

		/// <summary>
		/// The component
		/// </summary>
		public IBaseComponent Component { get; private set; }

		/// <summary>
		/// Horizontal center of rotation
		/// </summary>
		public double HorizontalRotationCenter => Component == null ? 0 : Component.Width / 2;

		/// <summary>
		/// Vertical center of rotation
		/// </summary>
		public double VerticalRotationCenter => Component == null ? 0 : Component.Height / 2;

		/// <summary>
		/// True if the component is focused (eg. pointer is over the element)
		/// </summary>
		public bool IsFocused { get; set; }

		#endregion

		#region Commands

		/// <summary>
		/// Engages focus on this component (shows its info, presents the highlight border, shows voltage drop and current
		/// flow direction
		/// </summary>
		public ICommand EngageFocusCommand { get; }

		/// <summary>
		/// Disengages focus
		/// </summary>
		public ICommand DisengageFocusCommand { get; }

		/// <summary>
		/// Command which puts the component into the edit menu thus starting to edit the component
		/// </summary>
		public ICommand EditComponentCommand { get; } = AppViewModel.Singleton.DesignVM.EditComponentCommand;

		/// <summary>
		/// Command for rotating the part 90 degrees left
		/// </summary>
		public ICommand RotateLeftCommand { get; }

		/// <summary>
		/// Command for rotating the part 90 degrees right
		/// </summary>
		public ICommand RotateRightCommand { get; }

		/// <summary>
		/// Command for removing this component from the circuit
		/// </summary>
		public ICommand RemoveComponentCommand { get; }

		/// <summary>
		/// Performs actions based on the current state.
		/// If idle, begins placing a new wire originating from the pressed socket
		/// If placing wire, ends placing the wire on the pressed socket.
		/// Parameter should be the pressed <see cref="IPartialNode"/>
		/// </summary>
		public ICommand SocketClickedCommand { get; }

		/// <summary>
		/// Reverses the voltage drop directions on the component
		/// </summary>
		public ICommand ReverseVoltageDropsCommand { get; }

		/// <summary>
		/// Increments the currently presented info section index (moves to the next info section)
		/// </summary>
		public ICommand ProceedToAnotherInfoSectionCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="ProceedToAnotherInfoSectionCommand"/>
		/// </summary>
		private void ProceedToAnotherInfoSection()
		{
			if (IsFocused)
			{
				++Component.ComponentInfo.CurrentSectionIndex;
			}
		}

		/// <summary>
		/// Method for <see cref="ReverseVoltageDropsCommand"/>
		/// </summary>
		private void ReverseVoltageDrops()
		{
			if (IsFocused)
			{
				Component.ChangeVIDirections = !Component.ChangeVIDirections;

				Component.UpdateInfo();
			}
		}

		/// <summary>
		/// Callback invoked when simulation completes, if the element is focused passes new info the the design view model
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SimulationCompletedCallback(object sender, SimulationCompletedEventArgs e)
		{
			if(IsFocused)
			{
				Component.UpdateInfo();
			}
		}

		/// <summary>
		/// Passes this component's info to the current DesignViewModel
		/// </summary>
		private void PassInfoToDesignVM() => AppViewModel.Singleton.DesignVM.ShowComponentInfo(Component.ComponentInfo);

		/// <summary>
		/// Method for <see cref="EngageFocusCommand"/>
		/// </summary>
		private void EngageFocus()
		{
			IsFocused = true;

			Component.UpdateInfo();

			PassInfoToDesignVM();
		}

		/// <summary>
		/// Method for <see cref="DisengageFocusCommand"/>
		/// </summary>
		private void DisengageFocus()
		{
			IsFocused = false;

			// Remove info from design view model
			AppViewModel.Singleton.DesignVM.HideComponentInfo();
		}

		/// <summary>
		/// Method for <see cref="RotateLeftCommand"/>
		/// </summary>
		private void RotateLeft() => Component.Rotate(90);

		/// <summary>
		/// Method for <see cref="RotateRightCommand"/>
		/// </summary>
		private void RotateRight() => Component.Rotate(-90);

		/// <summary>
		/// Method for <see cref="RemoveComponentCommand"/>
		/// </summary>
		private void RemoveComponent()
		{
			// If this component is currently edited
			if(AppViewModel.Singleton.DesignVM.ComponentEditSectionVM.CurrentlyEditedComponentViewModel != null &&
				AppViewModel.Singleton.DesignVM.ComponentEditSectionVM.CurrentlyEditedComponentViewModel.IsComponentEdited(Component))
			{
				// Stop editing
				AppViewModel.Singleton.DesignVM.ComponentEditSectionVM.CurrentlyEditedComponentViewModel = null;
			}

			// Unsubscribe from the simulation completed event
			IoC.Resolve<ISimulationManager>().SimulationCompleted -= SimulationCompletedCallback;

			// Remove the component from its schematic
			AppViewModel.Singleton.DesignVM.DesignManager.RemoveComponent(Component);

			Component = null;
		}

		/// <summary>
		/// Method for <see cref="SocketClickedCommand"/>
		/// </summary>
		private void SocketClicked(object parameter)
		{
			if (parameter is IPlanePosition position)
			{
				AppViewModel.Singleton.DesignVM.DesignManager.SocketClickedHandler(position);
			}
		}		

		#endregion
	}
}