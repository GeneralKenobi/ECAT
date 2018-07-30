using ECAT.Core;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of <see cref="IDefaultValues"/>, provides default values, allowed value ranges and similar
	/// values constant throughout the application
	/// </summary>
	public class DefaultValues : IDefaultValues
	{
		#region Public properties

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
		public Complex DefaultResistorAdmittance { get; } = new Complex(1e-3, 0);

		/// <summary>
		/// Default value for <see cref="ICurrentSource"/>'s produced current
		/// </summary>
		public double DefaultCurrentSourceProducedCurrent { get; } = 1e-3;

		/// <summary>
		/// Default value for <see cref="IVoltageSource"/>'s produced voltage
		/// </summary>
		public double DefaultVoltageSourceProducedVoltage { get; } = 5;

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