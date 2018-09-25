using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Inteface for base class for all components
	/// </summary>
	public interface IBaseComponent : INotifyPropertyChanged, IDisposable
    {
		#region Properties

		/// <summary>
		/// If true, voltage drop is calculated in reverse
		/// </summary>
		bool ChangeVIDirections { get; set; }

		/// <summary>
		/// The center of the component
		/// </summary>
		IPlanePosition Center { get; set; }

		/// <summary>
		/// Position of the handle of the component (top left corner)
		/// </summary>
		Complex Handle { get; }

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		double Width { get; }

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		double Height { get; }

		/// <summary>
		/// Radius of sockets on the component
		/// </summary>
		double SocketRadius { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Rotates the component by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees">Number of degrees to rotate the component by</param>
		void Rotate(double degrees);

		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		IEnumerable<ITerminal> GetTerminals();

		#endregion
	}
}