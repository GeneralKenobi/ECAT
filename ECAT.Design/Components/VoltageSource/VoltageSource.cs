﻿using ECAT.Core;
using CSharpEnhanced.CoreClasses;
using System.Numerics;
using System.Collections.Generic;
using System;
using System.Linq;
using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public class VoltageSource : TwoTerminal, IVoltageSource
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageSource() : base(new string[] { "Current", "Power" }) { }

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		#endregion
		
		#region Public properties		

		/// <summary>
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		public RefWrapperPropertyChanged<Complex> ProducedCurrent { get; set; } = new RefWrapperPropertyChanged<Complex>();

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of an <see cref="IVoltageSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		/// <summary>
		/// Returns info related to power
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<string> GetPowerInfo()
		{
			var dcPower = _VoltageDrop.Type.HasFlag(SignalType.DC) ? Math.Pow(_VoltageDrop.DC, 2) * GetAdmittance(0) : 0;

			var maxPower = Math.Pow(_VoltageDrop.Maximum, 2) * GetAdmittance(0);

			var rmsPower = _VoltageDrop.ComposingACWaveforms.Sum((voltage) =>
				(Math.Pow(voltage.Value.Magnitude, 2) * GetAdmittance(voltage.Key)).Magnitude) / 2 + dcPower;

			// Return characteristic power information
			yield return "Maximum instantenous power: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(maxPower.RoundToDigit(4), "W");

			yield return "Average power: " + SIHelpers.ToSIStringExcludingSmallPrefixes(rmsPower.RoundToDigit(4), "W");
		}

		/// <summary>
		/// Returns current info plus power info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo() =>
			new IEnumerable<string>[] { GetCurrentInfo(), GetPowerInfo() };

		#endregion
	}
}