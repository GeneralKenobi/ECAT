using System.Numerics;
using CSharpEnhanced.Maths;
using ECAT.Core;

namespace ECAT.Design
{
	public class OpAmp : ThreeTerminal, IOpAmp
	{
		public Variable PositiveSupplyVoltage { get; } = new Variable.VariableSource(15).Variable;
		public Variable NegativeSupplyVoltage { get; } = new Variable.VariableSource(-15).Variable;

		public override double Width { get; } = 300;

		public override double Height { get; } = 200;

		protected override Complex _TerminalAShift { get; } = new Complex(-150, 50);

		protected override Complex _TerminalBShift { get; } = new Complex(-150, -50);

		protected override Complex _TerminalCShift { get; } = new Complex(150, 0);
	}
}
