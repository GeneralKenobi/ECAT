using CSharpEnhanced.Maths;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for operational amplifiers - 
	/// </summary>
	public interface IOpAmp : IThreeTerminal
    {
		Variable PositiveSupplyVoltage { get; }
		Variable NegativeSupplyVoltage { get; }

		Variable OpenLoopGain { get; }
	}
}