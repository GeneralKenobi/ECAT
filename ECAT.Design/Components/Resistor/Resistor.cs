namespace ECAT.Design
{
	/// <summary>
	/// Class for a resistor in circuit design
	/// </summary>
    public class Resistor : TwoTerminal
    {
		double Resistance { get; set; } = 1000;
	}
}