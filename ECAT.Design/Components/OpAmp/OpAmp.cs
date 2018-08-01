using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	public class OpAmp : ThreeTerminal, IOpAmp
	{
		public override double Width { get; } = 300;

		public override double Height { get; } = 200;

		protected override Complex _TerminalAShift { get; } = new Complex(-150, 50);

		protected override Complex _TerminalBShift { get; } = new Complex(-150, -50);

		protected override Complex _TerminalCShift { get; } = new Complex(150, 0);
	}
}
