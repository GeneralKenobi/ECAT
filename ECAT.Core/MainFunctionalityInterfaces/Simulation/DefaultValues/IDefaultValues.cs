using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that defines default values for certain components (allowed ranges, fixed values, etc.)
	/// </summary>
	[NecessaryService]
	public interface IDefaultValues
	{
		#region Properties

		/// <summary>
		/// All coordinates are rounded to multiples of this value
		/// </summary>
		double RoundToCoordinates { get; }

		/// <summary>
		/// A maximum value for parameters in the circuit (admittance, voltage source voltage, etc)
		/// </summary>
		double MaximumParameterValue { get; }

		/// <summary>
		/// A minimum value for parameters in the circuit (resistance, reactance, voltage source voltage, etc)
		/// </summary>
		double MinimumParameterValue { get; }

		/// <summary>
		/// Admittance of a voltage source
		/// </summary>
		Complex VoltageSourceAdmittance { get; }

		/// <summary>
		/// Admittance of a current source
		/// </summary>
		Complex CurrentSourceAdmittance { get; }

		/// <summary>
		/// Default resistance for an <see cref="IResistor"/>
		/// </summary>
		double DefaultResistorResistance { get; }

		/// <summary>
		/// Default capacitance for an <see cref="ICapacitor"/>
		/// </summary>
		double DefaultCapacitorCapacitance { get; }

		/// <summary>
		/// Default frequency of an <see cref="IACVoltageSource"/>
		/// </summary>
		double DefaultACVoltageSourceFrequency { get; }

		/// <summary>
		/// Default peak voltage produced by an <see cref="IACVoltageSource"/>
		/// </summary>
		double DefaultACVoltageSourceProducedACVoltage { get; }

		/// <summary>
		/// Default value for <see cref="ICurrentSource"/>'s produced current
		/// </summary>
		double DefaultCurrentSourceProducedCurrent { get; }

		/// <summary>
		/// Default value for <see cref="IDCVoltageSource"/>'s produced voltage
		/// </summary>
		double DefaultVoltageSourceProducedVoltage { get; }

		/// <summary>
		/// Default value for <see cref="IOpAmp.PositiveSupplyVoltage"/>
		/// </summary>
		double DefaultOpAmpPositiveSupplyVoltage { get; }

		/// <summary>
		/// Default value for <see cref="IOpAmp.NegativeSupplyVoltage"/>
		/// </summary>
		double DefaultOpAmpNegativeSupplyVoltage { get; }

		/// <summary>
		/// Default value for <see cref="IOpAmp.OpenLoopGain"/>
		/// </summary>
		double DefaultOpAmpOpenLoopGain { get; }

		#endregion
	}
}