using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Resistor is a passive component that is characterized by resistance - a real component of impedance
	/// </summary>
	public class Resistor : TwoTerminal, IResistor
    {
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

		#endregion
	}
}