using CSharpEnhanced.ICommands;
using System;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// View model for the component edit menu
	/// </summary>
	public class ComponentEditSectionViewModel : BaseViewModel
    {
		#region Constructors

		/// <summary>
		/// Default constructor, assigns commands
		/// </summary>
		public ComponentEditSectionViewModel()
		{
			StopEditingCommand = new RelayCommand(StopEditing);
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever edited part changes
		/// </summary>
		public EventHandler<EditedComponentChangedEventArgs> EditedPartChangedEvent;

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="CurrentlyEditedComponentViewModel"/>
		/// </summary>		
		private BaseComponentEditViewModel mCurrentlyEditedComponentViewModel;

		#endregion

		#region Public properties
		
		/// <summary>
		/// Header for rotation section of the menu
		/// </summary>
		public string RotationSectionHeader { get; } = "Rotation";

		/// <summary>
		/// View model for the currently presented part (or null if nothing is presented)
		/// </summary>
		public BaseComponentEditViewModel CurrentlyEditedComponentViewModel
		{
			get => mCurrentlyEditedComponentViewModel;
			set
			{
				// If the value is identical, return
				if (mCurrentlyEditedComponentViewModel == value)
				{
					if (value != null)
					{
						EditedPartChangedEvent?.Invoke(this, new EditedComponentChangedEventArgs(EditedComponentChanged.NoChange));
					}

					return;
				}

				EditedComponentChanged changeType;

				// Determine the change type
				if(mCurrentlyEditedComponentViewModel == null)
				{
					// mCurrentEditViewModel and value can't be null at the same time due to the check above
					changeType = EditedComponentChanged.NullToComponent;
				}
				else if(value == null)
				{
					changeType = EditedComponentChanged.ComponentToNull;
				}
				else
				{
					changeType = EditedComponentChanged.ComponentToComponent;
				}

				// Assign the value
				mCurrentlyEditedComponentViewModel = value;

				// Fire the event
				EditedPartChangedEvent?.Invoke(this, new EditedComponentChangedEventArgs(changeType));
			}
		}

		/// <summary>
		/// Getter to the header of the edit menu (generic header if <see cref="CurrentlyEditedComponentViewModel"/> is null or
		/// current part's display name)
		/// </summary>
		public string HeaderName => CurrentlyEditedComponentViewModel == null ? "Component edit menu" :
			CurrentlyEditedComponentViewModel.EditedComponentDeclaration.DisplayName;

		/// <summary>
		/// When true, generic buttons (those that are present for every part, for example rotation buttons) should be enabled
		/// </summary>
		public bool EnableGenericButtons => CurrentlyEditedComponentViewModel != null;

		#endregion

		#region Commands

		/// <summary>
		/// Getter to the rotate left command from the edited part
		/// </summary>
		public ICommand RotateLeftCommand => CurrentlyEditedComponentViewModel?.RotateLeftCommand;

		/// <summary>
		/// Getter to the rotate right command from the edited part
		/// </summary>
		public ICommand RotateRightCommand => CurrentlyEditedComponentViewModel?.RotateRightCommand;

		/// <summary>
		/// Removes the currently edited part from the list of parts and consequently deletes it
		/// </summary>
		public ICommand RemoveCommand => CurrentlyEditedComponentViewModel?.RemoveComponentCommand;

		/// <summary>
		/// Command which stops editing the current part (removes its edit view model)
		/// </summary>
		public ICommand StopEditingCommand { get; private set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Stops editing the current part
		/// </summary>
		private void StopEditing() => CurrentlyEditedComponentViewModel = null;

		#endregion
	}
}