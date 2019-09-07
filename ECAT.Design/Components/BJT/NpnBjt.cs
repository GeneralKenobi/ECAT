using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Design
{
	[RegisterAsType(typeof(INpnBjt))]
	[DisplayVoltageInfo(nameof(TerminalC), nameof(TerminalA), 0, "Base-emitter voltage")]
	[DisplayVoltageInfo(nameof(TerminalC), nameof(TerminalB), 1, "Collector-emitter voltage")]
	public class NpnBjt : Bjt, INpnBjt
	{

	}
}
