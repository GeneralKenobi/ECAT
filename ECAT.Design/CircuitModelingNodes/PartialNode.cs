using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Class used to connect <see cref="BaseComponent"/> with
	/// </summary>
	public class PartialNode : IPartialNode
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public PartialNode() => Position = new PlanePosition();
		
		/// <summary>
		/// Constructor with parameters for shift values
		/// </summary>
		public PartialNode(double xShift, double yShift)
		{
			Position = new PlanePosition(0, 0, xShift, yShift);
		}

		/// <summary>
		/// Constructor with parameters for shift values
		/// </summary>
		public PartialNode(Complex shift)
		{
			Position = new PlanePosition(Complex.Zero, shift);
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// The coordinate of this PartialNode
		/// </summary>
		public IPlanePosition Position { get; }

		#endregion
	}
}