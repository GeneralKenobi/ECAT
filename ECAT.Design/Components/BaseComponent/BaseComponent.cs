using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all components
	/// </summary>
	public abstract class BaseComponent : IBaseComponent
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="Center"/>
		/// </summary>
		private IPlanePosition mCenter = new PlanePosition();

		#endregion

		#region Public properties

		/// <summary>
		/// If true, voltage drop is calculated from B to A (B is the reference node) instead of from A to B (A is the reference node)
		/// </summary>
		public bool ReverseVoltageDrops { get; set; }

		/// <summary>
		/// Radius of sockets on the component
		/// </summary>
		public double SocketRadius => 12;

		/// <summary>
		/// The center of the component
		/// </summary>
		public IPlanePosition Center
		{
			get => mCenter;
			set
			{
				if(mCenter != value)
				{
					if(mCenter != null)
					{
						mCenter.InternalStateChanged -= CenterChangedCallback;
					}

					mCenter = value;

					UpdateAbsoluteTerminalPositions();

					if(mCenter != null)
					{
						mCenter.InternalStateChanged += CenterChangedCallback;
					}
				}
			}
		}

		/// <summary>
		/// Position of the handle of the component (top left corner)
		/// </summary>
		public virtual Complex Handle => new Complex(Center.X - Width / 2, Center.Y + Height / 2);

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public abstract double Width { get; }

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public abstract double Height { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method used to call <see cref="UpdateAbsoluteTerminalPositions"/> whenever the <see cref="Center"/> changes or one of its values
		/// changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CenterChangedCallback(object sender, EventArgs e) => UpdateAbsoluteTerminalPositions();

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="PartialNode"/>s, invoked by <see cref="BaseComponent"/>'s constructor
		/// </summary>
		protected abstract void UpdateAbsoluteTerminalPositions();

		/// <summary>
		/// Rotates all partial nodes by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected abstract void RotateTerminals(double degrees);

		/// <summary>
		/// Invokes Property Changed Event for each string parameter
		/// </summary>
		/// <param name="propertyName">Properties to invoke for. Null or string.Empty will result in notification for all properties</param>
		protected void InvokePropertyChanged(params string[] propertyNames)
		{
			foreach (var item in propertyNames)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item));
			}
		}

		#endregion

		#region Public methods
		
		/// <summary>
		/// Returns the info that is to be presented, for example, on pointer over. It should include voltage drop(s) across the element,
		/// current(s) through the element, etc.
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<string> GetComponentInfo();
		
		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<ITerminal> GetTerminals();

		/// <summary>
		/// Rotates the component by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees">Number of degrees to rotate the component by</param>
		public void Rotate(double degrees)
		{
			Center.RotationAngle += degrees;

			RotateTerminals(degrees);
		}

		/// <summary>
		/// Disposes of the component
		/// </summary>
		public virtual void Dispose() { }

		#endregion
	}
}