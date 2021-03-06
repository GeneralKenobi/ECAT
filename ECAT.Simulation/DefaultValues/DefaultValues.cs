﻿using ECAT.Core;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of <see cref="IDefaultValues"/>, provides default values, allowed value ranges and similar
	/// values constant throughout the application
	/// </summary>
	[RegisterAsInstance(typeof(IDefaultValues))]
	public class DefaultValues : IDefaultValues
	{
		#region Public properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		public double RoundToCoordinates { get; } = 50;

		/// <summary>
		/// A maximum value for parameters in the circuit (admittance, voltage source voltage, etc)
		/// </summary>
		public double MaximumParameterValue { get; } = _MaximumParameterValue;

		/// <summary>
		/// A minimum value for parameters in the circuit (resistance, reactance, voltage source voltage, etc)
		/// </summary>
		public double MinimumParameterValue { get; } = 1 / _MaximumParameterValue;

		/// <summary>
		/// Admittance of a voltage source
		/// </summary>
		public Complex VoltageSourceAdmittance { get; } = Complex.Zero;

		/// <summary>
		/// Admittance of a current source
		/// </summary>
		public Complex CurrentSourceAdmittance { get; } = Complex.Zero;

		/// <summary>
		/// Default admittance for a <see cref="IResistor"/>
		/// </summary>
		public double DefaultResistorResistance { get; } = 1e3;

		/// <summary>
		/// Default capacitance for an <see cref="ICapacitor"/>
		/// </summary>
		public double DefaultCapacitorCapacitance { get; } = 1e-5;

		/// <summary>
		/// Default frequency of an <see cref="IACVoltageSource"/>
		/// </summary>
		public double DefaultACVoltageSourceFrequency { get; } = 1e1;

		/// <summary>
		/// Default peak voltage produced by an <see cref="IACVoltageSource"/>
		/// </summary>
		public double DefaultACVoltageSourceProducedACVoltage { get; } = 8;

		/// <summary>
		/// Default value for <see cref="ICurrentSource"/>'s produced current
		/// </summary>
		public double DefaultCurrentSourceProducedCurrent { get; } = 1e-3;

		/// <summary>
		/// Default value for <see cref="IDCVoltageSource"/>'s produced voltage
		/// </summary>
		public double DefaultVoltageSourceProducedVoltage { get; } = 5;

		/// <summary>
		/// Default value for <see cref="IOpAmp.PositiveSupplyVoltage"/>
		/// </summary>
		public double DefaultOpAmpPositiveSupplyVoltage { get; } = 15;

		/// <summary>
		/// Default value for <see cref="IOpAmp.NegativeSupplyVoltage"/>
		/// </summary>
		public double DefaultOpAmpNegativeSupplyVoltage { get; } = -15;

		/// <summary>
		/// Default value for <see cref="IOpAmp.OpenLoopGain"/>
		/// </summary>
		public double DefaultOpAmpOpenLoopGain { get; } = 1e5;

		#endregion

		#region Private static properties

		/// <summary>
		/// Maximum value that may be assigned to parameters, backing store for <see cref="MaximumParameterValue"/>,
		/// stored separately so as to be able to initialize public properties with it
		/// </summary>
		private static double _MaximumParameterValue { get; } = 1e+20;

		#endregion
	}
}