using Autofac;
using ECAT.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Default implementation of <see cref="IDesignManager"/>
	/// </summary>
	public class DesignManager : IDesignManager
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public DesignManager()
		{
			Schematics = new ReadOnlyObservableCollection<ISchematic>(_Schematics);
			ChangeCurrentSchematic(0);
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Backing store for <see cref="Schematic"/>
		/// </summary>
		private ObservableCollection<ISchematic> _Schematics { get; } = new ObservableCollection<ISchematic>()
		{
			new Schematic(),
		};

		/// <summary>
		/// Determines the direction of extending the wire
		/// </summary>
		private bool _ExtendWireAtEnd { get; set; } = true;

		/// <summary>
		/// The currently placed wire
		/// </summary>
		private IWire _PlacedWire { get; set; } = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// Collection of all active schematics
		/// </summary>
		public ReadOnlyObservableCollection<ISchematic> Schematics { get; }

		/// <summary>
		/// The currently edited schematic
		/// </summary>
		public ISchematic CurrentSchematic { get; private set; }

		/// <summary>
		/// True if the user is currently placing a wire
		/// </summary>
		public bool PlacingWire => _PlacedWire != null;

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public methods

		/// <summary>
		/// Handles clicks onto sockets
		/// </summary>
		/// <param name="node"></param>
		public void SocketClickedHandler(IPartialNode node)
		{
			// If there was a wire placed
			if (PlacingWire)
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
			if (!PlacingWire)
			{
				throw new InvalidOperationException("Can't add a point to the placed wire if there is no wire being placed");
			}

			_PlacedWire.AddPoint(position, _ExtendWireAtEnd);
		}

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
				RemoveWire(wire);

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
		/// Creates and adds a new, empty schematic
		/// </summary>
		public void AddSchematic() => _Schematics.Add(new Schematic());

		/// <summary>
		/// Adds the given schematic to <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		public void AddSchematic(ISchematic schematic) => _Schematics.Add(schematic);

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one given
		/// </summary>
		/// <param name="schematic"></param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="schematic"/> is not a part of <see cref="Schematics"/>
		/// </exception>
		public void ChangeCurrentSchematic(ISchematic schematic)
		{
			if(_Schematics.Contains(schematic))
			{
				CurrentSchematic = schematic;
			}
			else
			{
				throw new ArgumentException(nameof(schematic) + " is not a part of this " + nameof(IDesignManager));
			}
		}

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one at <paramref name="index"/> in the <see cref="Schematics"/>
		/// collection
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void ChangeCurrentSchematic(int index)
		{
			if(index < 0 || index >= _Schematics.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			CurrentSchematic = _Schematics[index];
		}

		/// <summary>
		/// Removes the given schematic from <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		public void RemoveSchematic(ISchematic schematic)
		{
			_Schematics.Remove(schematic);
			
			// If the removed schematic was the current one, set the current one to null
			if(CurrentSchematic == schematic)
			{
				CurrentSchematic = null;
			}
		}

		/// <summary>
		/// Removes schematic given by the argument
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void RemoveSchematic(int index)
		{
			if (index < 0 || index >= _Schematics.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			RemoveSchematic(_Schematics[index]);
		}

		/// <summary>
		/// Returns the type corresponding to the declaration
		/// </summary>
		/// <param name="declaration"></param>
		/// <returns></returns>
		public Type GetComponentType(IComponentDeclaration declaration) => GetComponentType(declaration.ID);

		/// <summary>
		/// Returns the type corresponding to the given component it
		/// </summary>
		/// <param name="declaration"></param>
		/// <returns></returns>
		public Type GetComponentType(int id)
		{
			switch(id)
			{
				case 0: return typeof(Resistor);

				default:
					{
						throw new ArgumentException($"No type corresponds to that ID: {id}");
					}
					
			}
		}

		/// <summary>
		/// Method that removes a component from its <see cref="ISchematic"/>. First searches through the <see cref="CurrentSchematic"/>,
		/// if the <paramref name="component"/> is not found there searches through the rest of <see cref="Schematics"/>
		/// </summary>
		/// <param name="component"></param>
		public void RemoveComponent(IBaseComponent component)
		{
			if(CurrentSchematic.Components.Contains(component))
			{
				CurrentSchematic.RemoveComponent(component);
			}
			else
			{
				foreach(var schematic in _Schematics)
				{
					if(schematic.Components.Contains(component))
					{
						schematic.RemoveComponent(component);
					}
				}
			}
		}

		/// <summary>
		/// Method that removes a wire from its <see cref="ISchematic"/>. First searches through the <see cref="CurrentSchematic"/>,
		/// if the <paramref name="wire"/> is not found there searches through the rest of <see cref="Schematics"/>
		/// </summary>
		/// <param name="wire"></param>
		public void RemoveWire(IWire wire)
		{
			if (CurrentSchematic.Wires.Contains(wire))
			{
				CurrentSchematic.RemoveWire(wire);
			}
			else
			{
				foreach (var schematic in _Schematics)
				{
					if (schematic.Wires.Contains(wire))
					{
						schematic.RemoveWire(wire);
					}
				}
			}
		}

		/// <summary>
		/// Finishes the wire placing procedure
		/// </summary>
		public void StopPlacingWire() => _PlacedWire = null;

		/// <summary>
		/// Creates and places a new loose wire on the given position
		/// </summary>
		/// <param name="position"></param>
		public void PlaceLooseWire(IPlanePosition position)
		{
			// Create a new wire
			CreateWireToPlace();

			// Add the position to it
			_PlacedWire.AddPoint(position);
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
			CurrentSchematic.AddWire(_PlacedWire);
		}

		#endregion
	}
}