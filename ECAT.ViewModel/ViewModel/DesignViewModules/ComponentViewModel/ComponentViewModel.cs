using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
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

			RotateLeftCommand = new RelayCommand(RotateLeft);
			RotateRightCommand = new RelayCommand(RotateRight);
			RemoveComponentCommand = new RelayCommand(RemoveComponent);
			SocketClickedCommand = new RelayParametrizedCommand(SocketClicked);
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
		/// Accessor to the <see cref="Component"/>'s info
		/// </summary>
		public IEnumerable<string> ComponentInfo => Component.GetComponentInfo();

		#endregion

		#region Commands

		/// <summary>
		/// Shows this component's info in the component info section on screen
		/// </summary>
		public ICommand ShowComponentInfoCommand { get; } = AppViewModel.Singleton.DesignVM.ShowComponentInfoCommand;

		/// <summary>
		/// Hides the component's info
		/// </summary>
		public ICommand HideComponentInfoCommand { get; } = AppViewModel.Singleton.DesignVM.HideComponentInfoCommand;

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

		#endregion

		#region Private methods

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