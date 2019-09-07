using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Implementation of n-channel Junction Field Effect Transistor
	/// </summary>
	[RegisterAsType(typeof(INChannelJfet))]
	public class NChannelJfet : Jfet, INChannelJfet
	{
	}
}
