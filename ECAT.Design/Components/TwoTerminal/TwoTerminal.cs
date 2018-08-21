using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
		protected TwoTerminal() : this(new string[] { "Voltage", "Current" }) { }

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected TwoTerminal(IEnumerable<string> headers) : base(headers)
		{
			TerminalA = new Terminal(new PlanePosition(Complex.Zero, _TerminalAShift));
			TerminalB = new Terminal(new PlanePosition(Complex.Zero, _TerminalBShift));
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

		/// <summary>
		/// Gets the voltage drop between nodes B and A (with A being the reference node)
		/// </summary>
		protected ISignalInformation _VoltageDrop => ReverseVoltageDrops ?
			IoC.Resolve<ISimulationResults>().GetVoltageDropOrZero(TerminalB.NodeIndex, TerminalA.NodeIndex) :
			IoC.Resolve<ISimulationResults>().GetVoltageDropOrZero(TerminalA.NodeIndex, TerminalB.NodeIndex);

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
		/// True if the standard voltage drop direciton (Vb - Va) was inverted
		/// </summary>
		public virtual bool InvertedVoltageCurrentDirections => _VoltageDrop.InvertedDirection;

		#endregion

		#region Private methods

		/// <summary>
		/// Returns AC currents for the current <see cref="_VoltageDrop"/>
		/// </summary>
		private List<Tuple<double, Complex>> GetAcCurrents() => new List<Tuple<double, Complex>>(
			_VoltageDrop.ComposingACWaveforms.Select((drop) =>
			new Tuple<double, Complex>(drop.Key, drop.Value * GetAdmittance(drop.Key))));

		/// <summary>		
		/// Returns DC current for the current <see cref="_VoltageDrop"/>
		/// </summary>
		private double GetDcCurrent() => _VoltageDrop.DC * GetAdmittance(0).Real;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns info related to voltage
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<string> GetVoltageInfo()
		{
			// Characteristic voltage drop information
			yield return "Maximum instantenous voltage: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(_VoltageDrop.Maximum.RoundToDigit(4), "V");
			yield return "Minimum instantenous voltage: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(_VoltageDrop.Minimum.RoundToDigit(4), "V");
			yield return "RMS voltage: " + SIHelpers.ToSIStringExcludingSmallPrefixes(_VoltageDrop.RMS.RoundToDigit(4), "V");

			// Notify the voltage drop direction may have changed
			InvokePropertyChanged(nameof(InvertedVoltageCurrentDirections));

			// DC voltage drop information
			if (_VoltageDrop.Type.HasFlag(SignalType.DC))
			{
				yield return "DC voltage: " + SIHelpers.ToSIStringExcludingSmallPrefixes(_VoltageDrop.DC.RoundToDigit(4), "V");
			}

			// AC voltage drop information
			if (_VoltageDrop.Type.HasFlag(SignalType.AC))
			{
				// If it's a multi-ac voltage waveform add a header
				if (_VoltageDrop.Type.HasFlag(SignalType.MultipleAC))
				{
					yield return "Composing AC waveforms:";
				}

				// Print each waveform
				foreach (var acWaveform in _VoltageDrop.ComposingACWaveforms)
				{
					yield return "AC voltage: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(acWaveform.Value.RoundToDigit(4), "V") +
						" at " + SIHelpers.ToSIStringExcludingSmallPrefixes(acWaveform.Key.RoundToDigit(4), "Hz");
				}
			}

		}

		/// <summary>
		/// Returns info related to current
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<string> GetCurrentInfo()
		{
			// Get ac and dc currents 
			var acCurrents = _VoltageDrop.Type.HasFlag(SignalType.AC) ? GetAcCurrents() : new List<Tuple<double, Complex>>();
			var dcCurrent = _VoltageDrop.Type.HasFlag(SignalType.DC) ? GetDcCurrent() : 0;

			// Calculate the characteristic values
			var maxCurrent = acCurrents.Sum((current) => current.Item2.Magnitude) + dcCurrent;
			var minCurrent = acCurrents.Sum((current) => -current.Item2.Magnitude) + dcCurrent;

			// RMS is a root square of a sum of squares of individual rms values (Magnitude divided by square root of 2 for AC)
			var rmsCurrent = Math.Sqrt(acCurrents.Sum((current) => Math.Pow(current.Item2.Magnitude, 2) / 2) + dcCurrent);

			// And return them
			yield return "Maximum instantenous current: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(maxCurrent.RoundToDigit(4), "A");
			yield return "Minimum instantenous current: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(minCurrent.RoundToDigit(4), "A");
			yield return "RMS current: " + SIHelpers.ToSIStringExcludingSmallPrefixes(rmsCurrent.RoundToDigit(4), "A");

			// Return DC current (if it's present)
			if (_VoltageDrop.Type.HasFlag(SignalType.DC))
			{
				yield return "DC current: " + SIHelpers.ToSIStringExcludingSmallPrefixes(
					dcCurrent.RoundToDigit(4), "A");
			}

			// Return AC current (if it's present)
			if (_VoltageDrop.Type.HasFlag(SignalType.AC))
			{
				// If it's a multi-ac voltage waveform add a header
				if (_VoltageDrop.Type.HasFlag(SignalType.MultipleAC))
				{
					yield return "Composing AC currents:";
				}

				// Print each waveform
				foreach (var current in acCurrents)
				{
					yield return "AC current: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
						current.Item2.RoundToDigit(4), "A") +
						" at " + SIHelpers.ToSIStringExcludingSmallPrefixes(current.Item1.RoundToDigit(4), "Hz");
				}
			}
		}

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

		/// <summary>
		/// Returns complete info for the component
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			yield return GetVoltageInfo();
			yield return GetCurrentInfo();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ITerminal> GetTerminals()	
		{
			yield return TerminalA;
			yield return TerminalB;
		}

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

		/// <summary>
		/// Returns conductance between <see cref="TerminalA"/> and <see cref="TerminalB"/> for DC signals. Conductance is the real part
		/// of admittance. For DC all elements have admittance with only real component being non-zero so this method returns the same
		/// as <see cref="GetAdmittance(double)"/> for 0 frequency.
		/// </summary>
		/// <returns></returns>
		public double GetConductance() => GetAdmittance(0).Real;

		#endregion
	}
}