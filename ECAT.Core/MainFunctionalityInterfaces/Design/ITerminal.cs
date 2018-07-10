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
		/// Node assigned to the element in the simulation, having it stored allows to display the results during real-time simulations
		/// </summary>
		INode Node { get; }
    }
}