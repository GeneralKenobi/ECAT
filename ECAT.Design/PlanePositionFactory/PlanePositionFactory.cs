using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Factory of <see cref="IPlanePosition"/>
	/// </summary>
	public class PlanePositionFactory : IPlanePositionFactory
	{
		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		public double RoundTo { get; } = 50;

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> with all coordinates 0
		/// </summary>
		/// <returns></returns>
		public IPlanePosition Construct() => new PlanePosition();

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based on two absolute coordinates
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public IPlanePosition Construct(double x, double y) => new PlanePosition(x, y);

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based on a <see cref="Complex"/> absolute coordinate
		/// </summary>
		/// <param name="absolute"></param>
		/// <returns></returns>
		public IPlanePosition Construct(Complex absolute) => new PlanePosition(absolute);

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based two absolute coordinates and two shift values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="xShift"></param>
		/// <param name="yShift"></param>
		/// <returns></returns>
		public IPlanePosition Construct(double x, double y, double xShift, double yShift) => new PlanePosition(x, y, xShift, yShift);

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based on two <see cref="Complex"/> coordinates - an absolute one and a shift
		/// </summary>
		/// <param name="absolute"></param>
		/// <param name="shift"></param>
		/// <returns></returns>
		public IPlanePosition Construct(Complex absolute, Complex shift) => new PlanePosition(absolute, shift);
	}
}