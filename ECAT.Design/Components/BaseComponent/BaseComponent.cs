using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all components
	/// </summary>
	public abstract class BaseComponent : IBaseComponent
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public BaseComponent()
		{
			UpdatePartialNodePositions();
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="RotationAngle"/>
		/// </summary>
		private double mRotationAngle = 0;

		/// <summary>
		/// Backing store for <see cref="Handle"/>
		/// </summary>
		private PlanePosition mHandle = new PlanePosition();

		#endregion

		#region Public properties

		/// <summary>
		/// Position of the handle of the component (top left corner)
		/// </summary>
		public PlanePosition Handle
		{
			get => mHandle;
			set
			{
				if(mHandle != value)
				{
					if(mHandle != null)
					{
						mHandle.InternalStateChanged -= HandleChangedCallback;
					}

					mHandle = value;

					UpdatePartialNodePositions();

					if(mHandle != null)
					{
						mHandle.InternalStateChanged += HandleChangedCallback;
					}
				}
			}
		}

		/// <summary>
		/// The center of the component
		/// </summary>
		public virtual cdouble Center => new cdouble(Handle.X - Width / 2, Handle.Y - Height / 2);

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public abstract double Width { get; }

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public abstract double Height { get; }

		/// <summary>
		/// Angle of component's rotation in degrees
		/// </summary>
		public double RotationAngle
		{
			get => mRotationAngle;
			set
			{
				var reducedValue = Helpers.ReduceAngle(value, AngleUnit.Degrees);

				if(reducedValue != mRotationAngle)
				{
					mRotationAngle = reducedValue;
					UpdatePartialNodePositions();
				}
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Disposes of the component
		/// </summary>
		public virtual void Dispose() { }

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="PartialNode"/>s, invoked by <see cref="BaseComponent"/>'s constructor
		/// </summary>
		protected abstract void UpdatePartialNodePositions();

		#endregion

		#region Private methods

		/// <summary>
		/// Method used to call <see cref="UpdatePartialNodePositions"/> whenever the <see cref="Handle"/> changes or one of its values
		/// changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleChangedCallback(object sender, EventArgs e) => UpdatePartialNodePositions();

		#endregion
	}
}