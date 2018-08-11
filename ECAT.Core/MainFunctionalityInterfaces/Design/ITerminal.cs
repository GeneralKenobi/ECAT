using CSharpEnhanced.CoreClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for terminals; terminal is an ending of an <see cref="IBaseComponent"/> that
	/// can be connected. It has an <see cref="IPlanePosition"/> and can be assigned an <see cref="RefWrapperPropertyChanged{T}"/>
	/// to have a real-time updated value of potential at the terminal
	/// </summary>
	public interface ITerminal : INotifyPropertyChanged
	{
		#region Events

		/// <summary>
		/// Event fired whenever the value of the potential changes
		/// </summary>
		EventHandler PotentialValueChanged { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potentials at <see cref="INode"/> that are associated with this <see cref="ITerminal"/>. Item1 (double) refers
		/// to the frequency of the source generating the potential and Item2 (Complex) to the value of the potential.
		/// </summary>
		IEnumerable<Tuple<double, Complex>> ACPotentials { get; set; }

		/// <summary>
		/// The DC potential of the terminal with respect to ground
		/// </summary>
		RefWrapper<Complex> DCPotential { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Maximum peak potential observable at the terminal
		/// </summary>
		/// <returns></returns>
		Complex MaximumPeakPotential();

		/// <summary>
		/// Minimum peak potential observable at the terminal
		/// </summary>
		/// <returns></returns>
		Complex MinimumPeakPotential();

		/// <summary>
		/// RMS value of voltage at the terminal
		/// </summary>
		/// <returns></returns>
		Complex RMSPotential();

		#endregion

	}
}