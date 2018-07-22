using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for wires for circuit design
	/// </summary>
	public interface IWire : INotifyPropertyChanged, IDisposable
	{
		#region Properties

		/// <summary>
		/// Radius of the sockets present on a wire
		/// </summary>
		double WireSocketRadius { get; }

		/// <summary>
		/// The beginning of the wire
		/// </summary>
		IPlanePosition Beginning { get; }

		/// <summary>
		/// The end of the wire
		/// </summary>
		IPlanePosition Ending { get; }

		/// <summary>
		/// Collection of all wires connected to this instance
		/// </summary>
		IList<IWire> ConnectedWires { get; set; }

		/// <summary>
		/// Collection of points that define the corners of the wire on the plane.
		/// Element at index 0 is a neighbour of <see cref="Beginning"/>, element at the last index is a neighbour of <see cref="Ending"/>
		/// </summary>
		ReadOnlyObservableCollection<IPlanePosition> DefiningPoints { get; }

		/// <summary>
		/// Collection of points that should be interpolated with a polyline to form a proper wire on the screen
		/// </summary>
		ReadOnlyObservableCollection<IPlanePosition> ConstructionPoints { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Merges this instance with <paramref name="wire"/>. After the method completes <paramref name="wire"/> is obselete.
		/// </summary>
		/// <param name="wire">Wire to merge into the instance on which the method was called</param>
		/// <param name="mergeToNode">Node of the instace on which the method was called that will assume the other end
		/// of <paramref name="wire"/></param>
		/// <param name="mergeFromNode">Node of <paramref name="wire"/> that will be lost (it will become an intermediate point)</param>
		void MergeWith(IWire wire, bool mergeToEnd, bool mergeFromEnd);

		/// <summary>
		/// Adds a new intermediate piont to the wire
		/// </summary>
		/// <param name="point"></param>
		void AddIntermediatePoint(IPlanePosition point);

		/// <summary>
		/// Adds a new point to the wire at beginning/end
		/// </summary>
		/// <param name="point"></param>
		void AddPoint(IPlanePosition point, bool addAtEnd = true);

		#endregion
	}
}