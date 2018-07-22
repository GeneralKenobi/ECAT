using ECAT.Core;
using System.Numerics;

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
   
		double Resistance { get; set; } = 1000;
	}
}