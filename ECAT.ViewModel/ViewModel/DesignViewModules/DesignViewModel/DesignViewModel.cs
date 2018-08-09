using Autofac;
using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for circuit design
	/// </summary>
	public class DesignViewModel : BaseViewModel
    {
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public DesignViewModel()
		{
			if(IoC.Container.TryResolve(out IDesignManager manager))
			{
				DesignManager = manager;
			}
			else
			{
				throw new Exception();
			}

			DesignManager.PropertyChanged += WireManipulated;

			StopActionCommand = new RelayCommand(StopAction);
			DesignAreaClickedCommand = new RelayParametrizedCommand(DesignAreaClicked);
			PrepareToPlaceLooseWireCommand = new RelayCommand(PrepareToPlaceLooseWire);
			EditComponentCommand = new RelayParametrizedCommand(EditComponent);
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="ComponentToAdd"/>
		/// </summary>
		private IComponentDeclaration mComponentToAdd;

		#endregion

		#region Private properties

		/// <summary>
		/// ID to use when logging messages to <see cref="IInfoLogger"/>
		/// </summary>
		private int _LoggerID { get; } = new Random().Next();

		/// <summary>
		/// Flag which, if set, ensures the next click on the design area will place a new wire in that position
		/// </summary>
		private bool _PlaceLooseWireOnNextClick { get; set; } = false;

		#endregion

		#region Public properties

		/// <summary>
		/// Indicates whether component info should be visible on the screen
		/// </summary>
		public bool ComponentInfoVisible => CurrentComponentInfo != null;

		/// <summary>
		/// Component info to currently present
		/// </summary>
		public IEnumerable<string> CurrentComponentInfo { get; private set; }

		/// <summary>
		/// InfoLogger for this app
		/// </summary>
		public IInfoLogger InfoLogger { get; } = IoC.Resolve<IInfoLogger>();

		/// <summary>
		/// Provided implementation of the <see cref="IDesignManager"/> interface
		/// </summary>
		public IDesignManager DesignManager { get; private set; }

		/// <summary>
		/// The component that was selected by the user to be added upon clicking on the screen.
		/// If null, then no component is to be added
		/// </summary>
		public IComponentDeclaration ComponentToAdd
		{
			get => mComponentToAdd;
			set
			{
				if(value != null && DesignManager.PlacingWire)
				{
					DesignManager.StopPlacingWire();
				}

				mComponentToAdd = value;

				if(mComponentToAdd == null)
				{
					InfoLogger.RemoveLog(_LoggerID);
				}
				else
				{
					InfoLogger.Log("Tap on the schematic to place a(n) " + mComponentToAdd.DisplayName, _LoggerID);
				}
			}
		}

		/// <summary>
		/// True if the user is currently adding components
		/// </summary>
		public bool AddingComponents => ComponentToAdd != null;

		/// <summary>
		/// View model for the component edit menu
		/// </summary>
		public ComponentEditSectionViewModel ComponentEditSectionVM { get; } = new ComponentEditSectionViewModel();

		#endregion

		#region Commands

		/// <summary>
		/// Stops the current action
		/// </summary>
		public ICommand StopActionCommand { get; }

		/// <summary>
		/// Command to invoke when the user clicks (presses, etc) the design area on the screen. The parameter should be the coordinate
		/// of the click given as a <see cref="PlanePosition"/>
		/// </summary>
		public ICommand DesignAreaClickedCommand { get; }

		/// <summary>
		/// Prepares the view model to place a loose wire - the next click on the design area will create it in the clicked position
		/// </summary>
		public ICommand PrepareToPlaceLooseWireCommand { get; }

		/// <summary>
		/// Puts a component in the ComponentEditMenu. Parameter should be the view model of the component to edit
		/// </summary>
		public ICommand EditComponentCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="EditComponentCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void EditComponent(object parameter)
		{
			if(parameter is ComponentViewModel vm)
			{
				// Use the helper class to assign a new view model for edit menu
				ComponentEditSectionVM.CurrentlyEditedComponentViewModel = DesignViewModelHelpers.ConstructAppropriateEditViewModel(vm);
			}
		}

		/// <summary>
		/// Method for <see cref="StopActionCommand"/>
		/// </summary>
		private void StopAction()
		{
			switch(AppViewModel.Singleton.State)
			{
				case AppState.AddingComponents:
					{
						ComponentToAdd = null;
					} break;

				case AppState.PlacingWire:
					{
						DesignManager.StopPlacingWire();
					} break;
			}
		}

		/// <summary>
		/// Method for <see cref="DesignAreaClickedCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void DesignAreaClicked(object parameter)
		{
			if (parameter is Complex position)
			{
				// Decide what to do with the input
				switch (AppViewModel.Singleton.State)
				{
					case AppState.AddingComponents:
						{
							AddComponent(IoC.Resolve<IPlanePositionFactory>().Construct(position));
						}
						break;

					case AppState.PlacingWire:
						{
							DesignManager.AddPointToPlacedWire(IoC.Resolve<IPlanePositionFactory>().Construct(position));
						}
						break;

					default:
						{
							if(_PlaceLooseWireOnNextClick)
							{
								DesignManager.PlaceLooseWire(IoC.Resolve<IPlanePositionFactory>().Construct(position));
							}
						} break;
				}
			}
		}

		/// <summary>
		/// Prepares the view-model to place a loose wire on the next click
		/// </summary>
		private void PrepareToPlaceLooseWire()
		{
			// The app needs to be in the idle state to allow for this quick action
			if (AppViewModel.Singleton.State == AppState.Idle)
			{
				// Set the flag
				_PlaceLooseWireOnNextClick = true;

				IoC.Log("Tap on the schematic to place a wire", _LoggerID);
			}
		}

		/// <summary>
		/// Listens to property changed on <see cref="DesignManager"/> and resets <see cref="_PlaceLooseWireOnNextClick"/> if
		/// the PlacingWire property changes (which means that there was some kind of wire manipulation that cancels the operation)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WireManipulated(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName==nameof(DesignManager.PlacingWire))
			{
				_PlaceLooseWireOnNextClick = false;
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Method for <see cref="HideComponentInfoCommand"/>
		/// </summary>
		public void HideComponentInfo() => CurrentComponentInfo = null;

		/// <summary>
		/// Method for <see cref="ShowComponentInfoCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		public void ShowComponentInfo(IEnumerable<string> info) => CurrentComponentInfo = info;

		/// <summary>
		/// Adds a component on the given position
		/// </summary>
		/// <param name="clickPosition"></param>
		public void AddComponent(IPlanePosition clickPosition)
		{
			if (AddingComponents)
			{
				var newComponent = IoC.Resolve<IComponentFactory>().Construct(ComponentToAdd);

				newComponent.Center = clickPosition;
				
				DesignManager.CurrentSchematic.AddComponent(newComponent);				
			}
		}		

		#endregion
	}
}