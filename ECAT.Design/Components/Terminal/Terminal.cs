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
		#region Constructors

		/// <summary>
		/// Default constructor taking position of the terminal as a parameter
		/// </summary>
		public Terminal(IPlanePosition position) => Position = position;

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
		/// The index of the corresponding node
		/// </summary>
		public int NodeIndex { get; set; }

		#endregion		
	}
}