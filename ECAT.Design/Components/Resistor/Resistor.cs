using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Resistor is a passive component that is characterized by resistance - a real component of impedance
	/// </summary>
	public class Resistor : TwoTerminal, IResistor
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public Resistor() : base(new string[] { "Voltage", "Current", "Power" }) { }

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="Resistance"/>
		/// </summary>
		private double mResistance = IoC.Resolve<IDefaultValues>().DefaultResistorResistance;

		#endregion

		#region Public properties

		/// <summary>
		/// The resistance of this <see cref="IResistor"/>
		/// </summary>
		public double Resistance
		{
			get => mResistance;
			set
			{
				if(value >= IoC.Resolve<IDefaultValues>().MinimumParameterValue)
				{
					mResistance = value;
				}
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of this resistor equal to the reciprocal of <see cref="Resistance"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => 1 / Resistance;

		/// <summary>
		/// Returns info related to power
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<string> GetPowerInfo()
		{
			// Calculate dc power
			var dcPower = _VoltageDrop.Type.HasFlag(VoltageDropType.DC) ? Math.Pow(_VoltageDrop.DC, 2) * GetAdmittance(0) : 0;

			// Calculate max power as maximum voltage drop squared times admittance
			var maxPower = Math.Pow(_VoltageDrop.Maximum, 2) * GetAdmittance(0);

			var rmsPower = _VoltageDrop.ComposingACWaveforms.Sum((voltage) =>
				(Math.Pow(voltage.Value.Magnitude, 2) * GetAdmittance(voltage.Key)).Magnitude) / 2 + dcPower;

			// Return characteristic power information
			yield return "Maximum instantenous power: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(maxPower.RoundToDigit(4), "W");

			yield return "Average power: " + SIHelpers.ToSIStringExcludingSmallPrefixes(rmsPower.RoundToDigit(4), "W");
		}

		/// <summary>
		/// Returns complete info for the component (basic two terminal plus power)
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo() =>
			base.GetComponentInfo().Concat(new IEnumerable<string>[] { GetPowerInfo() });

		#endregion
	}
}