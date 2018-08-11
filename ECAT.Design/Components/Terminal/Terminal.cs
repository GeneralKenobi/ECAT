using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

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

		/// <summary>
		/// Constructor taking position of the terminal as a parameter as well as a callback for when the value of the potential at
		/// the terminal changes (subscribes to <see cref="PotentialValueChanged"/> with it)
		/// </summary>
		/// <param name="position"></param>
		/// <param name="potentialValueChangedCallback"></param>
		public Terminal(IPlanePosition position, EventHandler potentialValueChangedCallback) : this(position) =>
			PotentialValueChanged += potentialValueChangedCallback;

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired whenever the value of the potential changes
		/// </summary>
		public EventHandler PotentialValueChanged { get; set; }

		#endregion

		#region Public properties

		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		public IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potential at <see cref="INode"/> that is associated with this <see cref="ITerminal"/>. Item1 (double) refers
		/// to the frequency of the source generating the potential and Item2 (Complex) to the value of the potential.
		/// </summary>
		public IEnumerable<Tuple<double, Complex>> ACPotentials { get; set; } = new List<Tuple<double, Complex>>();

		/// <summary>
		/// The DC potential of the terminal with respect to ground
		/// </summary>
		public RefWrapper<Complex> DCPotential { get; set; } = new RefWrapper<Complex>();

		#endregion

		#region Private methods

		/// <summary>
		/// Invokes the potential value changed event
		/// </summary>
		private void InvokePotentialValueChanged() => PotentialValueChanged?.Invoke(this, EventArgs.Empty);

		/// <summary>
		/// Propagates the property changed event of <see cref="ACPotentials"/> through <see cref="PotentialValueChanged"/>.
		/// There's only one property on <see cref="RefWrapperPropertyChanged{T}"/> so does not check for the name of the property
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PropagatePotenialValueChanged(object sender, PropertyChangedEventArgs e) => InvokePotentialValueChanged();

		#endregion

		#region Public methods

		/// <summary>
		/// Maximum peak potential observable at the terminal
		/// </summary>
		/// <returns></returns>
		public Complex MaximumPeakPotential()
		{			
			// Simply add all peak AC voltages to the DC potential
			var result = DCPotential.Value;

			foreach(var item in ACPotentials)
			{
				result += item.Item2;
			}

			return result;
		}

		/// <summary>
		/// Minimum peak potential observable at the terminal
		/// </summary>
		/// <returns></returns>
		public Complex MinimumPeakPotential()
		{
			// Simply subtract all peak AC voltages from the DC potential
			var result = DCPotential.Value;

			foreach (var item in ACPotentials)
			{
				result -= item.Item2;
			}

			return result;
		}

		/// <summary>
		/// RMS value of voltage at the terminal
		/// </summary>
		/// <returns></returns>
		public Complex RMSPotential()
		{
			// Total RMS is a square root of a sum of squares of RMS values of voltages present at the terminal
			var result = Complex.Pow(DCPotential.Value, 2);

			foreach (var item in ACPotentials)
			{
				result += Complex.Pow(item.Item2, 2);
			}

			return Complex.Sqrt(result);
		}

		#endregion
	}
}