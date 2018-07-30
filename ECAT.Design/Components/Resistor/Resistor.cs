using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Resistor is a passive component that is characterized by resistance - a real component of impedance
	/// </summary>
	public class Resistor : TwoTerminal, IResistor
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Resistor()
		{
			Admittance = IoC.Resolve<IDefaultValues>().DefaultResistorAdmittance;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The resistance of this <see cref="IResistor"/>
		/// </summary>
		public double Resistance
		{
			get => 1 / Admittance.Real;
			set
			{
				if(value >= IoC.Resolve<IDefaultValues>().MinimumParameterValue)
				{
					Admittance = 1 / value;
				}
				else
				{
					Admittance = IoC.Resolve<IDefaultValues>().MinimumParameterValue;
				}
			}
		}

		#endregion
	}
}