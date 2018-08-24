using System.Collections.Generic;
using System.Numerics;
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
		protected IEnumerable<string> GetPowerInfo(IPowerInformation powerInformation)
		{
			// Return characteristic power information
			yield return CIFormat.LineInfo("Maximum instantenous power", powerInformation.Maximum, "W");
			yield return CIFormat.LineInfo("Average power", powerInformation.Average, "W");
		}

		/// <summary>
		/// Returns complete info for the component (basic two terminal plus power)
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			yield return GetVoltageInfo(_VoltageDrop);
			yield return GetCurrentInfo(IoC.Resolve<ISimulationResults>().GetCurrent(_VoltageDrop, this));
			yield return GetPowerInfo(IoC.Resolve<ISimulationResults>().GetPower(_VoltageDrop, this));
		}

		#endregion
	}
}