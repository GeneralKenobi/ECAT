﻿using CSharpEnhanced.Maths;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for wires for circuit design
	/// </summary>
    public interface IWire : INotifyPropertyChanged, IDisposable
	{
		#region Properties

		/// <summary>
		/// The beginning of the wire
		/// </summary>
		IPartialNode N1 { get; set; }

		/// <summary>
		/// The end of the wire
		/// </summary>
		IPartialNode N2 { get; set; }

		/// <summary>
		/// Collection of all wires connected to this instance
		/// </summary>
		IList<IWire> ConnectedWires { get; set; }

		/// <summary>
		/// Collection of points that define the corners of the wire on the plane.
		/// Element at index 0 is a neighbour of <see cref="N1"/>, element at the last index is a neighbour of <see cref="N2"/>
		/// </summary>
		ReadOnlyObservableCollection<IPlanePosition> DefiningPoints { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Merges this instance with <paramref name="wire"/>. After the method completes <paramref name="wire"/> is obselete.
		/// </summary>
		/// <param name="wire">Wire to merge into the instance on which the method was called</param>
		/// <param name="mergeToNode">Node of the instace on which the method was called that will assume the other end
		/// of <paramref name="wire"/></param>
		/// <param name="mergeFromNode">Node of <paramref name="wire"/> that will be lost (it will become an intermediate point)</param>
		void MergeWith(IWire wire, IPartialNode mergeToNode, IPartialNode mergeFromNode);

		/// <summary>
		/// Adds a new intermediate piont to the wire
		/// </summary>
		/// <param name="point"></param>
		void AddIntermediatePoint(cdouble point);

		/// <summary>
		/// Adds a new point to the wire at beginning/end
		/// </summary>
		/// <param name="point"></param>
		void AddPoint(cdouble point, bool addAtEnd = true);

		#endregion
	}
}