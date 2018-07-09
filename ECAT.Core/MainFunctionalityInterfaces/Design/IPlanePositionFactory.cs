using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Factory of <see cref="IPlanePosition"/>
	/// </summary>
	public interface IPlanePositionFactory
    {
		#region Properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		double RoundTo { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> with all coordinates 0
		/// </summary>
		/// <returns></returns>
		IPlanePosition Construct();

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based on two absolute coordinates
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		IPlanePosition Construct(double x, double y);

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based on a <see cref="Complex"/> absolute coordinate
		/// </summary>
		/// <param name="absolute"></param>
		/// <returns></returns>
		IPlanePosition Construct(Complex absolute);

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based two absolute coordinates and two shift values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="xShift"></param>
		/// <param name="yShift"></param>
		/// <returns></returns>
		IPlanePosition Construct(double x, double y, double xShift, double yShift);

		/// <summary>
		/// Constructs an <see cref="IPlanePosition"/> based on two <see cref="Complex"/> coordinates - an absolute one and a shift
		/// </summary>
		/// <param name="absolute"></param>
		/// <param name="shift"></param>
		/// <returns></returns>
		IPlanePosition Construct(Complex absolute, Complex shift);

		#endregion
	}
}