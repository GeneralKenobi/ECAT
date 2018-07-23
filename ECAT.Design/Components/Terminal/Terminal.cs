using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Implementation of the <see cref="ITerminal"/> interface; terminal is an ending of an <see cref="IBaseComponent"/> that
	/// can be connected. It has an <see cref="IPlanePosition"/> and can be assigned an <see cref="RefWrapperPropertyChanged{T}"/>
	/// to have a real-time updated value of potential at the terminal
	/// </summary>
	public class Terminal : ITerminal
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Terminal(IPlanePosition position)
		{
			Position = position;
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		public IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potential at <see cref="INode"/> that is associated with this <see cref="ITerminal"/>
		/// </summary>
		public RefWrapperPropertyChanged<double> Potential { get; set; }

		#endregion
	}
}