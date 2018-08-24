using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all components
	/// </summary>
	public abstract class BaseComponent : IBaseComponent
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public BaseComponent()
		{
			_ComponentInfo = new ComponentInfo(Enumerable.Empty<string>());
		}

		/// <summary>
		/// Constructor with parameter used to construct <see cref="_ComponentInfo"/>
		/// </summary>
		public BaseComponent(IEnumerable<string> infoSectionsHeaders)
		{
			if(infoSectionsHeaders == null)
			{
				throw new ArgumentNullException(nameof(infoSectionsHeaders));
			}

			_ComponentInfo = new ComponentInfo(infoSectionsHeaders);
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
		/// Backing store for <see cref="Center"/>
		/// </summary>
		private IPlanePosition mCenter = new PlanePosition();

		#endregion

		#region Protected properties

		/// <summary>
		/// Backing store for <see cref="ComponentInfo"/>
		/// </summary>
		protected ComponentInfo _ComponentInfo { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// If true, voltage drop is calculated from B to A (B is the reference node) instead of from A to B (A is the reference node)
		/// Current is adjusted accordingly (instead of B to A, A to B)
		/// </summary>
		public bool ChangeVIDirections { get; set; }

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

		/// <summary>
		/// Info about this component (voltage drops, currents etc.) organized into subsections
		/// </summary>
		public IComponentInfo ComponentInfo => _ComponentInfo;

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
		/// Returns complete info for the component
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<IEnumerable<string>> GetComponentInfo();

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
		/// Updates <see cref="ComponentInfo"/>. Should be overriden if derived class provides info
		/// </summary>
		public void UpdateInfo() => _ComponentInfo.SetInfo(GetComponentInfo());
		
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