using CSharpEnhanced.Maths;
using ECAT.Core;
using System.ComponentModel;

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

		#region Public properties

		/// <summary>
		/// Position of the handle of the component (top left corner)
		/// </summary>
		public PlanePosition Handle { get; set; } = new PlanePosition();

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public abstract double Width { get; }

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public abstract double Height { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Disposes of the component
		/// </summary>
		public virtual void Dispose() { }

		#endregion
	}
}