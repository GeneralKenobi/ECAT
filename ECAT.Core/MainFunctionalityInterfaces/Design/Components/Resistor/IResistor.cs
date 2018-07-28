namespace ECAT.Core
{
	/// <summary>
	/// Interface for resistors - passive two-terminal component
	/// </summary>
	public interface IResistor : ITwoTerminal
    {
		/// <summary>
		/// The resistance of this <see cref="IResistor"/>
		/// </summary>
		double Resistance { get; set; }
    }
}