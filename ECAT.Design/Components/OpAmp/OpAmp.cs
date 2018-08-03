using System.Numerics;
using CSharpEnhanced.Maths;
using ECAT.Core;

namespace ECAT.Design
{
	public class OpAmp : ThreeTerminal, IOpAmp
	{
		public double PositiveSupplyVoltage { get; } = 15;
		public double NegativeSupplyVoltage { get; } = -15;
		public double OpenLoopGain { get; } = 1e6;

		public override double Width { get; } = 300;

		public override double Height { get; } = 200;

		protected override Complex _TerminalAShift { get; } = new Complex(-150, 50);

		protected override Complex _TerminalBShift { get; } = new Complex(-150, -50);

		protected override Complex _TerminalCShift { get; } = new Complex(150, 0);
	}
}
