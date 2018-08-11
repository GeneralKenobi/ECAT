﻿using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all two-terminal components
	/// </summary>
	public abstract class TwoTerminal : BaseComponent, ITwoTerminal
    {
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public TwoTerminal()
		{
			TerminalA = new Terminal(new PlanePosition(Complex.Zero, _TerminalAShift), TerminalPotentialChangedCallback);
			TerminalB = new Terminal(new PlanePosition(Complex.Zero, _TerminalBShift), TerminalPotentialChangedCallback);
			IoC.Resolve<ISimulationManager>().SimulationCompleted += (s, e) => InvokePropertyChanged(nameof(InvertedVoltageCurrentDirections));
		}

		#endregion

		#region Protected properties		

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalAShift => new Complex(-Width / 2, 0);

		/// <summary>
		/// The shift assigned to <see cref="TerminalB"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalBShift => new Complex(Width / 2, 0);

		#endregion

		#region Public properties		

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width => 200;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height => 100;

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public ITerminal TerminalB { get; }

		/// <summary>
		/// The maximum voltage drop that may be observed across the component
		/// </summary>
		public Complex MaximumVoltageDrop => TerminalB == null || TerminalA == null ? 0 : (InvertedVoltageCurrentDirections ?
			TerminalA.MaximumPeakPotential() - TerminalB.MaximumPeakPotential() :
			TerminalB.MinimumPeakPotential() - TerminalA.MinimumPeakPotential());

		/// <summary>
		/// The minimum voltage drop that may be observed across the component
		/// </summary>
		public Complex MinimumVoltageDrop => TerminalB == null || TerminalA == null ? 0 : (InvertedVoltageCurrentDirections ?
			TerminalA.MinimumPeakPotential() - TerminalB.MinimumPeakPotential() :
			TerminalB.MaximumPeakPotential() - TerminalA.MaximumPeakPotential());

		/// <summary>
		/// The RMS value of voltage across the component
		/// </summary>
		public Complex RMSVoltageDrop => TerminalB == null || TerminalA == null ? 0 : 
			Complex.Abs(TerminalB.RMSPotential() - TerminalA.RMSPotential());

		/// <summary>
		/// True if the standard voltage drop direciton (Vb - Va) was inverted
		/// </summary>
		public virtual bool InvertedVoltageCurrentDirections =>
			// Invert only if the maximum peak voltage would be negative
			TerminalA.MaximumPeakPotential().Real > TerminalB.MaximumPeakPotential().Real;
			

		/// <summary>
		/// Voltage drop across the part. The direction is determined by <see cref="InvertedVoltageCurrentDirections"/>
		/// </summary>
		public virtual Complex VoltageDrop => TerminalB.ACPotentials == null || TerminalA.ACPotentials == null ? 0 : TerminalB.MaximumPeakPotential() - TerminalA.MaximumPeakPotential();
			// Check if direction is inverted
			//(InvertedVoltageCurrentDirections ?
			// If it is calculate Va - Vb, otherwise Vb - Va
			//TerminalA.Potentials.Value - TerminalB.Potentials.Value : TerminalB.Potentials.Value - TerminalA.Potentials.Value);


		/// <summary>
		/// Current through the component, by convention (although not always as, for example, voltage sources will align current and
		/// voltage drop in the same direction. All things considered <see cref="VoltageDrop"/> and
		/// <see cref="InvertedVoltageCurrentDirections"/> are guaranteed to be mutually correct) it flows in direction opposite to voltage
		/// drop (so for <see cref="InvertedVoltageCurrentDirections"/> equal to false the current is given from <see cref="TerminalB"/> to
		/// <see cref="TerminalA"/>)
		public virtual Complex Current => VoltageDrop * GetAdmittance(0);
		// Current is simply calculated in the opposite direction to voltage

		#endregion

		#region Private methods

		/// <summary>
		/// Callback for when value of any <see cref="ITerminal"/> in this <see cref="TwoTerminal"/> changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TerminalPotentialChangedCallback(object sender, EventArgs e) =>
			InvokePropertyChanged(nameof(VoltageDrop), nameof(Current));

		#endregion

		#region Protected methods

		/// <summary>
		/// Assigns positions to all <see cref="ITerminal"/>s
		/// </summary>
		protected override void UpdateAbsoluteTerminalPositions()
		{
			TerminalA.Position.Absolute = new Complex(Center.X, Center.Y);
			TerminalB.Position.Absolute = new Complex(Center.X, Center.Y);
		}

		/// <summary>
		/// Rotates all partial nodes by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected override void RotateTerminals(double degrees)
		{
			TerminalA.Position.RotationAngle += degrees;
			TerminalB.Position.RotationAngle += degrees;
		}

		/// <summary>
		/// Returns admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>, called by <see cref="GetAdmittance(double)"/>
		/// </summary>
		protected abstract Complex CalculateAdmittance(double frequency);

		#endregion

		#region Public methods
				
		/// <summary>
		/// Returns the info that is to be presented, for example, on pointer over. It should include voltage drop(s) across the element,
		/// current(s) through the element, etc.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetComponentInfo()
		{
			yield return "Maximum instantenous voltage: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
				MaximumVoltageDrop.RoundToDigit(4), "V", imaginaryAsJ:true);
			yield return "Minimum instantenous voltage: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
				MinimumVoltageDrop.RoundToDigit(4), "V", imaginaryAsJ: true);
			yield return "RMS voltage: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
				RMSVoltageDrop.RoundToDigit(4), "V", imaginaryAsJ: true);
			yield return "Current: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(Current.RoundToDigit(4), "A", imaginaryAsJ:true);
		}

		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		public override List<ITerminal> GetTerminals() => new List<ITerminal>()
		{
			TerminalA,
			TerminalB,
		};

		/// <summary>
		/// Returns admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// </summary>
		public Complex GetAdmittance(double frequency)
		{
			// Check if the frequency is in the allowed range (nonnegative)
			if(frequency < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(frequency) + " cannot be negative");
			}

			// Call the helper method
			return CalculateAdmittance(frequency);
		}

		#endregion
	}
}