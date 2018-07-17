using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Resistor is a passive component that is characterized by resistance - a real component of impedance
	/// </summary>
    public class Resistor : TwoTerminal, IResistor
    {
		/// <summary>
		/// Default Constructor
		/// </summary>
		public Resistor()
		{
			Admittance = IoC.Resolve<IDefaultValues>().DefaultResistorAdmittance;
		}
	}
}