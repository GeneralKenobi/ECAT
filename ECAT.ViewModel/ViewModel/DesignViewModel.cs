using Autofac;
using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Generic;
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

			StopActionCommand = new RelayCommand(StopAction);
			DesignAreaClickedCommand = new RelayParametrizedCommand(DesignAreaClicked);
			PrepareToPlaceLooseWireCommand = new RelayCommand(PrepareToPlaceLooseWire);
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="_PlacedWire"/>
		/// </summary>
		private IWire mPlacedWire = null;

		#endregion

		#region Private properties

		/// <summary>
		/// Flag which, if set, ensures the next click on the design area will place a new wire in that position
		/// </summary>
		private bool _PlaceLooseWireOnNextClick { get; set; } = false;

		/// <summary>
		/// Determines the direction of extending the wire
		/// </summary>
		private bool _ExtendWireAtEnd { get; set; } = true;

		/// <summary>
		/// The currently placed wire
		/// </summary>
		private IWire _PlacedWire
		{
			get => mPlacedWire;
			set
			{
				// Whenever this property is set (either to some IWire or null), it means that wire manipulation has began/ended
				// and such action always cancels the action of placing a loose wire so reset the flag
				_PlaceLooseWireOnNextClick = false;

				mPlacedWire = value;
			}
		}

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

		/// <summary>
		/// True if the user is currently placing a wire
		/// </summary>
		public bool PlacingWire => _PlacedWire != null;

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
		/// Handles a click made on a wire socket
		/// </summary>
		/// <param name="wire"></param>
		/// <param name="endClicked"></param>
		public void WireSocketClickedHandler(IWire wire, bool endClicked)
		{
			// If there was a wire placed
			if (PlacingWire)
			{
				// Then it means the two wires need to be merged
				_PlacedWire.MergeWith(wire, endClicked, _ExtendWireAtEnd);

				// Remove the old wire
				DesignManager.RemoveWire(wire);

				// And remove the reference to the placed wire from the view-model
				_PlacedWire = null;
			}
			else
			{
				// Otherwise it means that the wire whose socket was pressed will be extended
				_PlacedWire = wire;

				// Assign the direction of extension
				_ExtendWireAtEnd = endClicked;
			}
		}

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

		/// <summary>
		/// Handles clicks onto sockets
		/// </summary>
		/// <param name="node"></param>
		public void SocketClickedHandler(IPartialNode node)
		{
			// If there was a wire placed
			if(PlacingWire)
			{
				// Then this action ends it; Assign the clicked node to the placed wire
				_PlacedWire.N2 = node;

				// And get rid of it's reference
				_PlacedWire = null;				
			}
			else
			{
				// Create a new wire
				CreateWireToPlace();

				// Assign it's initial node
				_PlacedWire.N1 = node;
			}
		}

		/// <summary>
		/// Adds a new point to the currently placed wire
		/// </summary>
		/// <param name="position"></param>
		/// <param name="addAtEnd"></param>
		public void AddPointToPlacedWire(IPlanePosition position)
		{
			if(!PlacingWire)
			{
				throw new InvalidOperationException("Can't add a point to the placed wire if there is no wire being placed");
			}

			_PlacedWire.AddPoint(position, _ExtendWireAtEnd);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Creates a new <see cref="IWire"/>, assigns it to <see cref="_PlacedWire"/>, adds it to the current schematic and marks
		/// that it's extended at the end
		/// </summary>
		private void CreateWireToPlace()
		{
			// Create a new wire
			_PlacedWire = IoC.Container.Resolve<IComponentFactory>().ConstructWire();

			// Signal that it's extended at the end
			_ExtendWireAtEnd = true;

			// Add the wire to the current schematic
			DesignManager.CurrentSchematic.AddWire(_PlacedWire);
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
						_PlacedWire = null;
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
							AddPointToPlacedWire(position);
						}
						break;

					default:
						{
							if(_PlaceLooseWireOnNextClick)
							{
								PlaceLooseWire(position);
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
		/// Creates and places a new loose wire on the given position
		/// </summary>
		/// <param name="position"></param>
		private void PlaceLooseWire(IPlanePosition position)
		{
			// Create a new wire
			CreateWireToPlace();

			// Add the position to it
			_PlacedWire.AddPoint(position);
		}

		#endregion
	}
}