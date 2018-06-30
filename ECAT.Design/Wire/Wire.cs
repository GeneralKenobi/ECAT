using CSharpEnhanced.Maths;
using ECAT.Design;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Provides means to connect two <see cref="PartialNode"/>s as well as an arbitrary number of <see cref="WireNode"/>s.
	/// Later before running the simulation dedicated logic will scan all connections and merge them into a single <see cref="Node"/>
	/// </summary>
    public class Wire : INotifyPropertyChanged, IDisposable
    {
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// The first end of the wire (may be floating)
		/// </summary>
		public PartialNode N1 { get; set; }

		/// <summary>
		/// The second end of the wire (may be floating)
		/// </summary>
		public PartialNode N2 { get; set; }

		/// <summary>
		/// List with all wires that are connected to this wire somewhere in the middle
		/// </summary>
		public List<Wire> ConnectedWires { get; set; }

		/// <summary>
		/// Collection of points that define the intermediate points of the wire. Point indexed 0 is the neighbour of <see cref="N1"/>,
		/// point at the last index is the neighbour of <see cref="N2"/>
		/// </summary>
		[DoNotNotify]
		public ObservableCollection<cdouble> ControlPoints { get; set; } = new ObservableCollection<cdouble>();

		#endregion

		#region Public methods

		/// <summary>
		/// Disposes of the wire (disconnets itself from all other wires)
		/// </summary>
		public void Dispose() => ConnectedWires.ForEach((wire) => wire.ConnectedWires.Remove(this));

		/// <summary>
		/// Merges this instance with <paramref name="wire"/>. After the call completes <paramref name="wire"/> is obselete.
		/// </summary>
		/// <param name="wire"></param>
		public void MergeWith(Wire wire, PartialNode mergeToNode, PartialNode mergeFromNode)
		{
			// Swap all connections to the new wire
			wire.ConnectedWires.ForEach((x) => x.ConnectedWires[x.ConnectedWires.FindIndex((y) => y == wire)] = this);

			// Clear the connections from the old wire
			wire.ConnectedWires.Clear();

			// Concat the control points
			if (mergeToNode == N1)
			{
				// If the wire is merged at the beginning, then to the beginning				
				ControlPoints = new ObservableCollection<cdouble>(wire.ControlPoints.Concat(ControlPoints));
			}
			else
			{
				// If the wire is merged at the end, then to the end
				ControlPoints = new ObservableCollection<cdouble>(ControlPoints.Concat(wire.ControlPoints));
			}
						
			wire.ControlPoints.Clear();

			// Assign the new beginning (or end) of the wire
			mergeToNode = (wire.N1 == mergeFromNode ? wire.N2 : wire.N1);
		}

		#endregion
	}
}