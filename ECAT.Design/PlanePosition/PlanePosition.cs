using CSharpEnhanced.Maths;
using ECAT.Core;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Position on a plane that can be shifted without losing the absolute center coordinate and rotated around it
	/// </summary>
	[RegisterAsType(typeof(IPlanePosition))]
	public class PlanePosition : IPlanePosition
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public PlanePosition()
		{
			InternalStateChanged += InternalStateChnagedCallback;
		}

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
		public PlanePosition(Complex absolute) : this() => mAbsolute = absolute.RoundTo(RoundTo);

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

		#region Private members

		/// <summary>
		/// Backing store for <see cref="_Shift"/>
		/// </summary>
		private Complex mShift;

		/// <summary>
		/// Backing store for <see cref="Absolute"/>
		/// </summary>
		private Complex mAbsolute;

		/// <summary>
		/// Backing store for <see cref="RotationAngle"/>
		/// </summary>
		private double mRotationAngle;

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="Shift"/>
		/// </summary>
		private Complex _Shift
		{
			get => mShift;
			set => mShift = value.RoundTo(RoundTo);
		}		

		#endregion

		#region Public Properties

		/// <summary>
		/// Angle of component's rotation in degrees
		/// </summary>
		public double RotationAngle
		{
			get => mRotationAngle;
			set
			{
				var reducedValue = MathsHelpers.ReduceAngle(value, AngleUnit.Degrees);

				if (reducedValue != mRotationAngle)
				{
					// To rotate to the desired angle we need to compensate for the already applied rotation
					var difference = reducedValue - mRotationAngle;

					// Assign the new rotation angle
					mRotationAngle = reducedValue;

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
			get => mAbsolute;
			set
			{
				if(mAbsolute != value)
				{
					mAbsolute = value.RoundTo(RoundTo);

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
		[DoNotNotify]
		public double X => Absolute.Real + Shift.Real;

		/// <summary>
		/// Final Y coordinate of this position (with applied shift)
		/// </summary>
		[DoNotNotify]
		public double Y => Absolute.Imaginary + Shift.Imaginary;

		#endregion
		
		#region Private methods

		/// <summary>
		/// Invokes the <see cref="InternalStateChanged"/> event
		/// </summary>
		private void InvokeInternalStateChanged() => InternalStateChanged?.Invoke(this, EventArgs.Empty);

		/// <summary>
		/// Callback for internal state changed, invokes property changed event for <see cref="X"/> and <see cref="Y"/>
		/// </summary>
		private void InternalStateChnagedCallback(object sender, EventArgs e)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Creates and returns a deep clone of the instance
		/// </summary>
		/// <returns></returns>
		public IPlanePosition DeepClone() => new PlanePosition(Absolute.Real, Absolute.Imaginary, Shift.Real, Shift.Imaginary);

		/// <summary>
		/// Returns true if this and <paramref name="position"/> are at the same position
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool Equals(IPlanePosition position) => X == position.X && Y == position.Y;

		/// <summary>
		/// Returns true if this and <paramref name="position"/> are at the same position
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool Equals(Complex position) => X == position.Real && Y == position.Imaginary;

		/// <summary>
		/// Returns a string representation of the position
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"({X}, {Y})";

		#endregion

		#region Public static properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		public static double RoundTo { get; } = IoC.Resolve<IDefaultValues>().RoundToCoordinates;

		#endregion
	}
}