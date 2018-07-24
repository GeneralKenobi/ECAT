using System.Collections.Generic;
using ECAT.Core;

namespace ECAT.Design
{
	public class Ground : BaseComponent, IOneTerminal
	{
		public override double Width { get; } = 50;

		public override double Height { get; } = 50;

		public override List<ITerminal> GetTerminals()
		{
			throw new System.NotImplementedException();
		}

		protected override void RotatePartialNodes(double degrees)
		{
			throw new System.NotImplementedException();
		}

		protected override void UpdateAbsolutePartialNodePositions()
		{
			throw new System.NotImplementedException();
		}
	}
}
