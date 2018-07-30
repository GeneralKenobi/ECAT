using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ECAT.Design
{
	/// <summary>
	/// Provides means to connect two <see cref="PartialNode"/>s as well as an arbitrary number of <see cref="WireNode"/>s.
	/// Later before running the simulation dedicated logic will scan all connections and merge them into a single <see cref="Node"/>
	/// </summary>
	public class Wire : IWire
    {
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Wire()
		{
			_DefiningPoints.CollectionChanged += DefiningPointsChanged;
			DefiningPoints = new ReadOnlyObservableCollection<IPlanePosition>(_DefiningPoints);
			ConstructionPoints = new ReadOnlyObservableCollection<IPlanePosition>(_ConstructionPoints);

			_ConstructionPoints.CollectionChanged += OnConstructionPointsChanged;
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private properties

		/// <summary>
		/// Radius of the sockets present on a wire
		/// </summary>
		public double WireSocketRadius { get; } = 12;

		/// <summary>
		/// Backing store for <see cref="DefiningPoints"/>
		/// </summary>
		private ObservableCollection<IPlanePosition> _DefiningPoints { get; } = new ObservableCollection<IPlanePosition>();
		
		/// <summary>
		/// Backing store for <see cref="ConstructionPoints"/>
		/// </summary>
		private ObservableCollection<IPlanePosition> _ConstructionPoints { get; } = new ObservableCollection<IPlanePosition>();
				
		#endregion

		#region Public properties

		/// <summary>
		/// The first end of the wire
		/// </summary>
		public IPlanePosition Beginning => _ConstructionPoints.Count > 0 ? _ConstructionPoints[0] : new PlanePosition();

		/// <summary>
		/// The second end of the wire
		/// </summary>
		public IPlanePosition Ending => _ConstructionPoints.Count > 0 ?
				_ConstructionPoints[_ConstructionPoints.Count - 1] : new PlanePosition();

		/// <summary>
		/// Collection of points that define the intermediate points of the wire. Point indexed 0 is the neighbour of <see cref="Beginning"/>,
		/// point at the last index is the neighbour of <see cref="Ending"/>
		/// </summary>
		public ReadOnlyObservableCollection<IPlanePosition> DefiningPoints { get; }

		/// <summary>
		/// Collection of points that should be interpolated with a polyline to form a proper wire on the screen
		/// </summary>
		public ReadOnlyObservableCollection<IPlanePosition> ConstructionPoints { get; }

		#endregion

		#region Private methods		

		/// <summary>
		/// Returns true if the point given by <paramref name="position"/> belongs to wire but is not in construction points yet,
		/// assigns the index of its preceding construction point to <paramref name="index"/>. If the point doesn't belong to wire
		/// returns false and assigns -1 to <paramref name="index"/>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private bool FindPrecidingConstructionPoint(IPlanePosition position, out int index)
		{
			for (int i = 0; i < ConstructionPoints.Count - 1; ++i)
			{
				// If the point lies on the line between two subsequent points
				if ((ConstructionPoints[i].X == position.X && ConstructionPoints[i + 1].X == position.X &&
					(position.Y - ConstructionPoints[i].Y) * (position.Y - ConstructionPoints[i + 1].Y) < 0) ||
					(ConstructionPoints[i].Y == position.Y && ConstructionPoints[i + 1].Y == position.Y &&
					(position.X - ConstructionPoints[i].X) * (position.X - ConstructionPoints[i + 1].X) < 0))
				{
					index = i;
					return true;
				}
			}

			index = -1;
			return false;
		}

		/// <summary>
		/// Gets all <see cref="IWire"/>s connected to this <see cref="IWire"/>
		/// </summary>
		/// <param name="alreadyFoundWires"></param>
		/// <returns></returns>
		private List<IWire> GetConnectedWiresRecursively(List<IWire> alreadyFoundWires, IEnumerable<IWire> allWires)
		{
			// Add this instance to already found wires
			alreadyFoundWires.Add(this);			

			// For each wire in the collection of all wires
			foreach(var wire in allWires)
			{
				// If it's not in the already found wires and either of its ends lies on this wire or either of this wire's
				// ends lies on the inspected wire
				if (!alreadyFoundWires.Contains(wire) && (BelongsToWire(wire.Beginning) || BelongsToWire(wire.Ending) ||
					wire.BelongsToWire(Beginning) || wire.BelongsToWire(Ending)))
				{
					// Add all of the wire's connected wires to the already found wires list, additionally if it's also a Wire
					// use this private method
					alreadyFoundWires.AddRange(wire is Wire castedWire ?
						castedWire.GetConnectedWiresRecursively(alreadyFoundWires, allWires) : wire.GetAllConnectedWires(allWires));
				}
			}

			return alreadyFoundWires;
		}

		/// <summary>
		/// Adds a point to the end of the wire, adds necessary points to <see cref="_ConstructionPoints"/>
		/// </summary>
		/// <param name="point"></param>
		private void AddPointAtEnd(IPlanePosition point)
		{
			_DefiningPoints.Add(point);

			if (_ConstructionPoints.Count > 0)
			{
				_ConstructionPoints.Add(new PlanePosition(point.X, _ConstructionPoints[_ConstructionPoints.Count - 1].Y));
			}

			_ConstructionPoints.Add(point);
		}

		/// <summary>
		/// Adds a point to the beginning of the wire, adds necessary points to <see cref="_ConstructionPoints"/>
		/// </summary>
		/// <param name="point"></param>
		private void AddPointAtBeginning(IPlanePosition point)
		{
			_DefiningPoints.Insert(0, point);

			if (_ConstructionPoints.Count > 0)
			{
				_ConstructionPoints.Insert(0, new PlanePosition(point.X, _ConstructionPoints[0].Y));
			}

			_ConstructionPoints.Insert(0, point);
		}

		/// <summary>
		/// Callback for changes in <see cref="_DefiningPoints"/>, raises <see cref="PropertyChanged"/> on <see cref="DefiningPoints"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefiningPointsChanged(object sender, NotifyCollectionChangedEventArgs e) => PropertyChanged?.Invoke(
			this, new PropertyChangedEventArgs(nameof(DefiningPoints)));

		/// <summary>
		/// Callback for ConstructionPointsChanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConstructionPointsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Beginning)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ending)));
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns true if point given by <paramref name="position"/> belongs to this wire
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool BelongsToWire(IPlanePosition position) => FindPrecidingConstructionPoint(position, out var dummy) ||
			ConstructionPoints.FirstOrDefault((constructionPoint) => position.Equals(constructionPoint)) != null;

		/// <summary>
		/// Returns an IList of all wires connected to this <see cref="IWire"/> (including connections through other wires)
		/// </summary>
		/// <returns></returns>
		public IList<IWire> GetAllConnectedWires(IEnumerable<IWire> allWires) =>
			// Use the private helper method, get only distinct elements
			new List<IWire>(GetConnectedWiresRecursively(new List<IWire>(), allWires).Distinct());			

		/// <summary>
		/// Disposes of the wire
		/// </summary>
		public void Dispose()
		{
			
		}

		/// <summary>
		/// Merges this instance with <paramref name="wire"/>. After the call completes <paramref name="wire"/> is obselete.
		/// </summary>
		/// <param name="wire"></param>
		public void MergeWith(IWire wire, bool mergeToEnd, bool mergeFromEnd)
		{
			if (mergeFromEnd)
			{
				// If the wire is merged from end

				// The defining points need to be added starting from the end
				for (int i = wire.DefiningPoints.Count - 1; i >= 0; --i)
				{
					// If the merging is made to the end, then the points need to be added to the end, otherwise to the beginning
					_DefiningPoints.Insert(mergeToEnd ? _DefiningPoints.Count : 0, wire.DefiningPoints[i]);
				}

				// Now for the construction points (to keep the original shape they'll be transfered over from the merged wire)
				
				// First of all insert a point that will join merged ends of both wires
				// Add the point to the merged to end
				_ConstructionPoints.Insert(mergeToEnd ? _ConstructionPoints.Count : 0,
					// Take the X from the merged to end
					new PlanePosition(_ConstructionPoints[mergeToEnd ? _ConstructionPoints.Count - 1 : 0].X,
					// And take Y from the merged from end
					wire.ConstructionPoints[wire.ConstructionPoints.Count - 1].Y));

				// Now for the rest of the points
				for (int i = wire.ConstructionPoints.Count - 1; i >= 0; --i)				
				{
					// If the merging is made to the end, then the points need to be added to the end, otherwise to the beginning
					_ConstructionPoints.Insert(mergeToEnd ? _ConstructionPoints.Count : 0, wire.ConstructionPoints[i]);
				}
			}
			else
			{
				// This case is the same except the loops are iterated from 0 to max

				for (int i = 0; i < wire.DefiningPoints.Count; ++i)
				{
					_DefiningPoints.Insert(mergeToEnd ? _DefiningPoints.Count : 0, wire.DefiningPoints[i]);
				}

				_ConstructionPoints.Insert(mergeToEnd ? _ConstructionPoints.Count : 0,
					new PlanePosition(_ConstructionPoints[mergeToEnd ? _ConstructionPoints.Count - 1 : 0].X,
					// And here the y coordinate is taken from the beginning (the merged from end)
					wire.ConstructionPoints[0].Y));

				for (int i = 0; i < wire.ConstructionPoints.Count; ++i)
				{
					_ConstructionPoints.Insert(mergeToEnd ? _ConstructionPoints.Count : 0, wire.ConstructionPoints[i]);
				}
			}

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConstructionPoints)));
		}

		/// <summary>
		/// Adds a new intermediate piont to the wire
		/// </summary>
		/// <param name="point"></param>
		public void AddIntermediatePoint(IPlanePosition point)
		{
			// If there are not enough coords to add intermediate points, throw an exception
			if (DefiningPoints.Count < 2)
			{
				throw new Exception("Can't add an intermediate point to a wire with less than 2 coords");
			}

			// If the point that we add is already a construction point return
			if(ConstructionPoints.FirstOrDefault((position) => position.Equals(point)) != null)
			{
				return;
			}

			// If the point is not included in the wire as a construction point but it does lie on the wire
			if (FindPrecidingConstructionPoint(point, out int index))
			{
				// Add it to the wire's construction points
				_ConstructionPoints.Insert(index + 1, point);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConstructionPoints)));
				return;
			}

			// Otherwise signal error
			throw new Exception("The given position doesn't lie on the wire");
		}

		/// <summary>
		/// Adds a new point to the wire at beginning/end
		/// </summary>
		/// <param name="point"></param>
		public void AddPoint(IPlanePosition point, bool addAtEnd = true)
		{
			if(addAtEnd)
			{
				AddPointAtEnd(point);
			}
			else
			{
				AddPointAtBeginning(point);
			}

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConstructionPoints)));
		}

		#endregion
	}
}