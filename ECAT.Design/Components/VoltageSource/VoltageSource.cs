using ECAT.Core;
using Autofac;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (well almost ideal because the internal admittance can't
	/// be set to infinity - it would not work in the calculations, however it is set to a very big value - 1e100
	/// which corresponds to 1e-100 resistance - practically 0)
	/// </summary>
    public class VoltageSource : TwoTerminal, IVoltageSource
    {

		/// <summary>
		/// Default Constructor
		/// </summary>
		public VoltageSource()
		{
			Admittance = IoC.Container.Resolve<IDefaultValues>().MaximumParameterValue;
		}
    }
}