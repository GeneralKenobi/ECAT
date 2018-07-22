using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that defines default values for certain components (allowed ranges, fixed values, etc.)
	/// </summary>
    public interface IDefaultValues
    {
		#region Properties

		/// <summary>
		/// A maximum value for parameters in the circuit (admittance, voltage source voltage, etc)
		/// </summary>
		double MaximumParameterValue { get; }

		/// <summary>
		/// Admittance of a voltage source
		/// </summary>
		Complex VoltageSourceAdmittance { get; }

		/// <summary>
		/// Admittance of a current source
		/// </summary>
		Complex CurrentSourceAdmittance { get; }

		/// <summary>
		/// Default admittance for a <see cref="IResistor"/>
		/// </summary>
		Complex DefaultResistorAdmittance { get; }

		#endregion
	}
}