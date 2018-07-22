using Autofac;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.ComponentModel;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Position on a plane that can be shifted without losing the absolute center coordinate and rotated around it
	/// </summary>
	public class PlanePosition : IPlanePosition
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public PlanePosition() { }

		/// <summary>
		/// Constructor with absolute position parameters
		/// </summary>
		/// <param name="x">Absolute horizontal position</param>
		/// <param name="y">Absolute vertical position</param>
		public PlanePosition(double x, double y) : this(new Complex(x, y)) { }

		/// <summary>
		/// Constructor with absolute position and shift parameters
		/// </summary>
		/// <param name="x">Absolute horizontal position</param>
		/// <param name="y">Absolute vertical position</param>
		/// <param name="xShift">Horizontal shift</param>
		/// <param name="yShift">Vertical shift</param>
		public PlanePosition(double x, double y, double xShift, double yShift) :
			this(new Complex(x, y), new Complex(xShift, yShift)) { }

		/// <summary>
		/// Constructor with absolute position parameter
		/// </summary>
		/// <param name="absolute"></param>
		public PlanePosition(Complex absolute) => _Absolute = absolute.RoundTo(RoundTo);

		/// <summary>
		/// Constructor with absolute as well as shift parameters
		/// </summary>
		/// <param name="absolute"></param>
		/// <param name="shift"></param>
		public PlanePosition(Complex absolute, Complex shift) : this(absolute) => _Shift = shift.RoundTo(RoundTo);

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;		

		/// <summary>
		/// Event fired when the internal state of this Position changes	
		/// </summary>
		public event EventHandler InternalStateChanged;	

		#endregion

		#region Private properties
		
		/// <summary>
		/// Backing store for <see cref="Absolute"/>
		/// </summary>
		private Complex _Absolute { get; set; }

		/// <summary>
		/// Backign store for <see cref="Shift"/>
		/// </summary>
		private Complex _Shift { get; set; }

		/// <summary>
		/// Backing store for <see cref="RotationAngle"/>
		/// </summary>
		private double _RotationAngle { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Angle of component's rotation in degrees
		/// </summary>
		public double RotationAngle
		{
			get => _RotationAngle;
			set
			{				
				var reducedValue = Helpers.ReduceAngle(value, AngleUnit.Degrees);

				if (reducedValue != _RotationAngle)
				{
					// To rotate to the desired angle we need to compensate for the already applied rotation
					var difference = reducedValue - _RotationAngle;

					// Assign the new rotation angle
					_RotationAngle = value;

					// If the shift is 0 then rotation won't change anyting - skip it
					if (_Shift.Magnitude != 0)
					{
						_Shift *= Complex.FromPolarCoordinates(1, Math.PI * difference / 180);
					}

					InvokeInternalStateChanged();					
				}

			}
		}

		/// <summary>
		/// The absolute coord of this position
		/// </summary>
		public Complex Absolute
		{
			get => _Absolute;
			set
			{
				if(_Absolute != value)
				{
					_Absolute = value.RoundTo(RoundTo);

					InvokeInternalStateChanged();					
				}
			}
		}

		/// <summary>
		/// The shift applied to this position
		/// </summary>
		public Complex Shift => _Shift;		

		/// <summary>
		/// Final X coordinate of this position (with applied shift)
		/// </summary>
		public double X => Absolute.Real + Shift.Real;

		/// <summary>
		/// Final Y coordinate of this position (with applied shift)
		/// </summary>
		public double Y => Absolute.Imaginary + Shift.Imaginary;

		#endregion

		#region Public static properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		public static double RoundTo { get; } = IoC.Resolve<IPlanePositionFactory>().RoundTo;

		#endregion

		#region Private methods

		/// <summary>
		/// Invokes the <see cref="InternalStateChanged"/> event
		/// </summary>
		private void InvokeInternalStateChanged() => InternalStateChanged?.Invoke(this, EventArgs.Empty);

		#endregion

		#region Public methods

		/// <summary>
		/// Creates and returns a deep clone of the instance
		/// </summary>
		/// <returns></returns>
		public IPlanePosition DeepClone() => new PlanePosition(Absolute.Real, Absolute.Imaginary, Shift.Real, Shift.Imaginary);

		#endregion
	}
}