using System;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Inteface for base class for all components
	/// </summary>
	public interface IBaseComponent : INotifyPropertyChanged, IDisposable
    {
		#region Properties

		/// <summary>
		/// Position of the handle of the component (top left corner)
		/// </summary>
		PlanePosition Handle { get; set; }

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		double Width { get; }

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		double Height { get; }

		/// <summary>
		/// Angle of component's rotation in degrees
		/// </summary>
		double RotationAngle { get; set; }

		#endregion
	}
}