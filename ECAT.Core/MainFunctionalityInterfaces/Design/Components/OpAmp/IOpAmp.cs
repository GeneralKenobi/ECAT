using CSharpEnhanced.Maths;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for operational amplifiers - 
	/// </summary>
	public interface IOpAmp : IThreeTerminal
    {
		double PositiveSupplyVoltage { get; }
		double NegativeSupplyVoltage { get; }

		double OpenLoopGain { get; }
	}
}