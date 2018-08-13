using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for terminals; terminal is an ending of an <see cref="IBaseComponent"/> that
	/// can be connected. It has an <see cref="IPlanePosition"/> and can be assigned an <see cref="RefWrapperPropertyChanged{T}"/>
	/// to have a real-time updated value of potential at the terminal
	/// </summary>
	public interface ITerminal : INotifyPropertyChanged
	{
		#region Properties

		/// <summary>
		/// The index of the corresponding node
		/// </summary>
		int NodeIndex { get; set; }

		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		IPlanePosition Position { get; }		

		#endregion
	}
}