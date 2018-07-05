﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class acting as a coordinate
	/// </summary>
    public interface IPlanePosition : INotifyPropertyChanged
    {
		#region Events

		/// <summary>
		/// Event fired when the internal state changes
		/// </summary>
		event EventHandler InternalStateChanged;

		#endregion

		#region Properties

		/// <summary>
		/// Angle of rotation in degrees
		/// </summary>
		double RotationAngle { get; set; }

		/// <summary>
		/// The absolute coord of this position
		/// </summary>
		Complex Absolute { get; set; }

		/// <summary>
		/// The shift applied to this position
		/// </summary>
		Complex Shift { get; }

		/// <summary>
		/// Final X coordinate of this position (with applied shift)
		/// </summary>
		double X { get; }

		/// <summary>
		/// Final Y coordinate of this position (with applied shift)
		/// </summary>
		double Y { get; }

		#endregion
	}
}
