using Autofac;
using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for circuit design
	/// </summary>
    public class DesignViewModel : BaseViewModel
    {
		#region Constructor

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
		}

		#endregion		

		#region Private properties

		/// <summary>
		/// Flag which, if set, ensures the next click on the design area will place a new wire in that position
		/// </summary>
		private bool _PlaceLooseWireOnNextClick { get; set; } = false;		

		#endregion

		#region Public properties		

		/// <summary>
		/// Provided implementation of the <see cref="IDesignManager"/> interface
		/// </summary>
		public IDesignManager DesignManager { get; private set; }

		/// <summary>
		/// The component that was selected by the user to be added upon clicking on the screen.
		/// If null, then no component is to be added
		/// </summary>
		public IComponentDeclaration ComponentToAdd { get; set; }

		/// <summary>
		/// True if the user is currently adding components
		/// </summary>
		public bool AddingComponents => ComponentToAdd != null;		

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

		#endregion

		#region Public methods		

		/// <summary>
		/// Adds a component on the given position
		/// </summary>
		/// <param name="clickPosition"></param>
		public void AddComponent(IPlanePosition clickPosition)
		{
			if (AddingComponents)
			{
				var newComponent = IoC.Container.Resolve<IComponentFactory>().Construct(ComponentToAdd);

				newComponent.Center = clickPosition;
				
				DesignManager.CurrentSchematic.AddComponent(newComponent);				
			}
		}		

		#endregion

		#region Private methods

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
			if (parameter is IPlanePosition position)
			{
				// Decide what to do with the input
				switch (AppViewModel.Singleton.State)
				{
					case AppState.AddingComponents:
						{
							AddComponent(position);
						}
						break;

					case AppState.PlacingWire:
						{
							DesignManager.AddPointToPlacedWire(position);
						}
						break;

					default:
						{
							if(_PlaceLooseWireOnNextClick)
							{
								DesignManager.PlaceLooseWire(position);
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
	}
}