using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Class for a resistor in circuit design
	/// </summary>
    public class Resistor : TwoTerminal
    {
		double Resistance { get; set; } = 1000;

		/// <summary>
		/// Admittance between terminals A and B
		/// </summary>
		public override Complex Admittance => new Complex(Resistance, 0);
	}
}