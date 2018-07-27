using CSharpEnhanced.CoreClasses;
using System;
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
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potential at <see cref="INode"/> that is associated with this <see cref="ITerminal"/>
		/// </summary>
		RefWrapperPropertyChanged<double> Potential { get; set; }

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever the value of the potential changes
		/// </summary>
		EventHandler PotentialValueChanged { get; set; }

		#endregion
	}
}