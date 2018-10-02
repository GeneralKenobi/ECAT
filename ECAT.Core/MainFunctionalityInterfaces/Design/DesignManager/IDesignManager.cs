using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for design of a circuit
	/// </summary>
	[NecessaryService]
	public interface IDesignManager : INotifyPropertyChanged
    {
		#region Properties

		/// <summary>
		/// Collection of all active schematics
		/// </summary>
		ReadOnlyObservableCollection<ISchematic> Schematics { get; }

		/// <summary>
		/// The currently edited schematic
		/// </summary>
		ISchematic CurrentSchematic { get; }

		/// <summary>
		/// True if a wire is being placed
		/// </summary>
		bool PlacingWire { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Creates and adds a new, empty schematic
		/// </summary>
		void AddSchematic();

		/// <summary>
		/// Adds the given schematic to <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		void AddSchematic(ISchematic schematic);

		/// <summary>
		/// Removes the given schematic from <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		void RemoveSchematic(ISchematic schematic);

		/// <summary>
		/// Removes schematic given by the argument
		/// </summary>
		/// <param name="index"></param>
		void RemoveSchematic(int index);

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one given
		/// </summary>
		/// <param name="schematic"></param>
		void ChangeCurrentSchematic(ISchematic schematic);

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one at <paramref name="index"/> in the <see cref="Schematics"/> collection
		/// </summary>
		/// <param name="index"></param>
		void ChangeCurrentSchematic(int index);

		/// <summary>
		/// Method that removes a component from its <see cref="ISchematic"/>. First searches through the <see cref="CurrentSchematic"/>,
		/// if the <paramref name="component"/> is not found there searches through the rest of <see cref="Schematics"/>
		/// </summary>
		/// <param name="component"></param>
		void RemoveComponent(IBaseComponent component);

		/// <summary>
		/// Method that removes a wire from its <see cref="ISchematic"/>. First searches through the <see cref="CurrentSchematic"/>,
		/// if the <paramref name="wire"/> is not found there searches through the rest of <see cref="Schematics"/>
		/// </summary>
		/// <param name="wire"></param>
		void RemoveWire(IWire wire);

		/// <summary>
		/// Handles clicks onto sockets
		/// </summary>
		/// <param name="position"></param>
		void SocketClickedHandler(IPlanePosition position);

		/// <summary>
		/// Adds a new point to the currently placed wire
		/// </summary>
		/// <param name="position"></param>
		/// <param name="addAtEnd"></param>
		void AddPointToPlacedWire(IPlanePosition position);

		/// <summary>
		/// Handles a click made on a wire socket
		/// </summary>
		/// <param name="wire"></param>
		/// <param name="endClicked"></param>
		void WireSocketClickedHandler(IWire wire, bool endClicked);

		/// <summary>
		/// Finishes the wire placing procedure
		/// </summary>
		void StopPlacingWire();

		/// <summary>
		/// Creates and places a new, loose wire on the given position
		/// </summary>
		void PlaceLooseWire(IPlanePosition position);

		/// <summary>
		/// Handles clicks performed on a wire
		/// </summary>
		/// <param name="wire"></param>
		void WireClickedHandler(IWire wire, IPlanePosition clickPosition);

		#endregion
	}
}