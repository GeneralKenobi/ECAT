using Autofac;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ECAT.Design
{
	/// <summary>
	/// Provides means to connect two <see cref="PartialNode"/>s as well as an arbitrary number of <see cref="WireNode"/>s.
	/// Later before running the simulation dedicated logic will scan all connections and merge them into a single <see cref="Node"/>
	/// </summary>
	public class Wire : IWire
    {
		#region Constructor

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
		public double WireSocketRadius => 12;

		/// <summary>
		/// Backing store for <see cref="DefiningPoints"/>
		/// </summary>
		private ObservableCollection<IPlanePosition> _DefiningPoints { get; } = new ObservableCollection<IPlanePosition>();
		
		/// <summary>
		/// Backing store for <see cref="ConstructionPoints"/>
		/// </summary>
		private ObservableCollection<IPlanePosition> _ConstructionPoints { get; } = new ObservableCollection<IPlanePosition>();

		/// <summary>
		/// Backing store for <see cref="ConnectedWires"/>
		/// </summary>
		private List<IWire> _ConnectedWires { get; set; } = new List<IWire>();

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
		/// List with all wires that are connected to this wire somewhere in the middle
		/// </summary>
		public IList<IWire> ConnectedWires => _ConnectedWires;

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

		#region Public methods

		/// <summary>
		/// Returns an IList of all wires connected to this <see cref="IWire"/> (including connections through other wires, excluding
		/// this instance)
		/// </summary>
		/// <returns></returns>
		public IList<IWire> GetAllConnectedWires() =>
			// Use the private helper method, filter out this instance and get only distinct elements
			new List<IWire>(GetConnectedWiresRecursively(new List<IWire>()).Where((wire) => wire != this).Distinct());

		/// <summary>
		/// Disposes of the wire (disconnets itself from all other wires)
		/// </summary>
		public void Dispose()
		{
			foreach(var wire in ConnectedWires)
			{
				wire.ConnectedWires.Remove(this);
			}
		}

		/// <summary>
		/// Merges this instance with <paramref name="wire"/>. After the call completes <paramref name="wire"/> is obselete.
		/// </summary>
		/// <param name="wire"></param>
		public void MergeWith(IWire wire, bool mergeToEnd, bool mergeFromEnd)
		{
			// Swap all connections to the new wire
			foreach (var x in wire.ConnectedWires)
			{
				x.ConnectedWires[x.ConnectedWires.IndexOf(wire)] = this;
			}

			// Clear the connections from the old wire
			wire.ConnectedWires.Clear();
						
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

			// If the point that we add is already on the wire just return
			if(ConstructionPoints.FirstOrDefault((position) => position.Equals(point)) != null)
			{
				return;
			}

			for (int i = 0; i < ConstructionPoints.Count - 1; ++i)
			{
				// If the point lies on the line between two subsequent points
				if ((ConstructionPoints[i].X == point.X && ConstructionPoints[i + 1].X == point.X &&
					(point.Y - ConstructionPoints[i].Y) * (point.Y - ConstructionPoints[i + 1].Y) < 0) ||
					(ConstructionPoints[i].Y == point.Y && ConstructionPoints[i + 1].Y == point.Y &&
					(point.X - ConstructionPoints[i].X) * (point.X - ConstructionPoints[i + 1].X) < 0))
				{
					// Add it to the wire's construction points
					_ConstructionPoints.Insert(i + 1, point);
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConstructionPoints)));
					return;
				}
			}

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

		#region Private methods

		/// <summary>
		/// Gets all <see cref="IWire"/>s connected to this <see cref="IWire"/>
		/// </summary>
		/// <param name="alreadyFoundWires"></param>
		/// <returns></returns>
		private List<IWire> GetConnectedWiresRecursively(List<IWire> alreadyFoundWires)
		{
			// Make a list of results, including this instance to prevent the wires connected to it from quering it
			List<IWire> result = new List<IWire>() { this, };

			// For each connected wire
			_ConnectedWires.ForEach((wire) =>
			{
				// If it's not in the already found wires
				if (!alreadyFoundWires.Contains(wire))
				{
					// If it's a Wire, query it and give it a list of all connected wires, if it's only an IWire then use the
					// public method to obtain all connected wires
					result.AddRange(wire is Wire castedWire ?
						castedWire.GetConnectedWiresRecursively(result) : wire.GetAllConnectedWires());
				}
			});

			return result;
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
	}
}