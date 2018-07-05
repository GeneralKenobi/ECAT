using CSharpEnhanced.Maths;
using ECAT.Core;
using ECAT.Design;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
			_DefiningPoints = new ObservableCollection<IPlanePosition>();
			DefiningPoints = new ReadOnlyObservableCollection<IPlanePosition>(_DefiningPoints);
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="_DefiningPoints"/>
		/// </summary>
		private ObservableCollection<IPlanePosition> mDefiningPoints;

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="N1"/>
		/// </summary>
		private IPartialNode _N1 { get; set; }

		/// <summary>
		/// Backing store for <see cref="N2"/>
		/// </summary>
		private IPartialNode _N2 { get; set; }

		/// <summary>
		/// Backing store for <see cref="DefiningPoints"/>
		/// </summary>
		private ObservableCollection<IPlanePosition> _DefiningPoints
		{
			get => mDefiningPoints;
			set
			{
				if(mDefiningPoints != value)
				{
					if(mDefiningPoints != null)
					{
						mDefiningPoints.CollectionChanged -= DefiningPointsChanged;
					}

					mDefiningPoints = value;

					if(mDefiningPoints != null)
					{
						mDefiningPoints.CollectionChanged += DefiningPointsChanged;
					}
				}
			}
		}
	
		#endregion

		#region Public properties

		/// <summary>
		/// The first end of the wire (may be floating)
		/// </summary>
		public IPartialNode N1
		{
			get => _N1;
			set
			{
				if(_N1 != value)
				{
					if(_N1 != null)
					{
						_DefiningPoints.Remove(_N1.Position);
					}

					_N1 = value;

					if(_N1 != null)
					{
						_DefiningPoints.Insert(0, _N1.Position);
						_N1.Position.InternalStateChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefiningPoints)));
					}
				}
			}
		}

		/// <summary>
		/// The second end of the wire (may be floating)
		/// </summary>
		public IPartialNode N2
		{
			get => _N2;
			set
			{
				if (_N2 != value)
				{
					if (_N2 != null)
					{
						_DefiningPoints.Remove(_N2.Position);
					}

					_N2 = value;

					if (_N2 != null)
					{
						_DefiningPoints.Insert(_DefiningPoints.Count - 1, _N2.Position);
						_N2.Position.InternalStateChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefiningPoints)));
					}
				}
			}
		}

		/// <summary>
		/// List with all wires that are connected to this wire somewhere in the middle
		/// </summary>
		public IList<IWire> ConnectedWires { get; set; }

		/// <summary>
		/// Collection of points that define the intermediate points of the wire. Point indexed 0 is the neighbour of <see cref="N1"/>,
		/// point at the last index is the neighbour of <see cref="N2"/>
		/// </summary>
		public ReadOnlyObservableCollection<IPlanePosition> DefiningPoints { get; }

		#endregion

		#region Public methods

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
		public void MergeWith(IWire wire, IPartialNode mergeToNode, IPartialNode mergeFromNode)
		{
			// Swap all connections to the new wire
			foreach (var x in wire.ConnectedWires)
			{
				x.ConnectedWires[x.ConnectedWires.IndexOf(wire)] = this;
			}

			// Clear the connections from the old wire
			wire.ConnectedWires.Clear();

			// Concat the control points
			if (mergeToNode == N1)
			{				
				// If the wire is merged at the beginning, then to the beginning				
				_DefiningPoints = new ObservableCollection<IPlanePosition>(wire.DefiningPoints.Concat(DefiningPoints));
			}
			else
			{
				// If the wire is merged at the end, then to the end
				_DefiningPoints = new ObservableCollection<IPlanePosition>(DefiningPoints.Concat(wire.DefiningPoints));
			}

			// Assign the new beginning (or end) of the wire
			mergeToNode = (wire.N1 == mergeFromNode ? wire.N2 : wire.N1);
		}

		/// <summary>
		/// Adds a new intermediate piont to the wire
		/// </summary>
		/// <param name="point"></param>
		public void AddIntermediatePoint(cdouble point)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds a new point to the wire at beginning/end
		/// </summary>
		/// <param name="point"></param>
		public void AddPoint(cdouble point, bool addAtEnd = true)
		{
			int index = addAtEnd ? _DefiningPoints.Count - 1 : 0;

			_DefiningPoints.Insert(index, new PlanePosition(point.Re, point.Im));
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Callback for changes in <see cref="_DefiningPoints"/>, raises <see cref="PropertyChanged"/> on <see cref="DefiningPoints"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefiningPointsChanged(object sender, NotifyCollectionChangedEventArgs e) => PropertyChanged?.Invoke(
			this, new PropertyChangedEventArgs(nameof(DefiningPoints)));

		#endregion
	}
}