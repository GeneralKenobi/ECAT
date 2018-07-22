using CSharpEnhanced.CoreClasses;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for terminals - each terminal has a position and allows for 
	/// </summary>
	public interface ITerminal : INotifyPropertyChanged
    {
		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potential at <see cref="INode"/> that is associated with this <see cref="ITerminal"/>
		/// </summary>
		RefWrapper<double> Potential { get; set; }
    }
}