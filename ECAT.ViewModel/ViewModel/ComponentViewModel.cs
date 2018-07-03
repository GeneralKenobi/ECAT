using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for components in circuit design
	/// </summary>
    public class ComponentViewModel : BaseViewModel
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComponentViewModel(IBaseComponent component)
		{
			Component = component ?? throw new ArgumentNullException(nameof(component) + " cannot be null");

			RotateLeftCommand = new RelayCommand(RotateLeft);
			RotateRightCommand = new RelayCommand(RotateRight);
			RemoveComponentCommand = new RelayCommand(RemoveComponent);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The component 
		/// </summary>
		public IBaseComponent Component { get; }

		/// <summary>
		/// Horizontal center of rotation
		/// </summary>
		public double HorizontalRotationCenter => Component.Width / 2;

		/// <summary>
		/// Vertical center of rotation
		/// </summary>
		public double VerticalRotationCenter => Component.Height / 2;

		/// <summary>
		/// X coordinate of the center of the component
		/// </summary>
		public double CenterX => Component.Handle == null ? 0 : Component.Handle.X - Component.Width / 2;

		/// <summary>
		/// Y coordinate of the center of the component
		/// </summary>
		public double CenterY => Component.Handle == null ? 0 : Component.Handle.Y - Component.Height / 2;

		#endregion

		#region Commands

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

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="RotateLeftCommand"/>
		/// </summary>
		private void RotateLeft() => Component.RotationAngle += 90;

		/// <summary>
		/// Method for <see cref="RotateRightCommand"/>
		/// </summary>
		private void RotateRight() => Component.RotationAngle -= 90;

		/// <summary>
		/// Method for <see cref="RemoveComponentCommand"/>
		/// </summary>
		private void RemoveComponent() => AppViewModel.Singleton.DesignVM.DesignManager.RemoveComponent(Component);

		#endregion
	}
}