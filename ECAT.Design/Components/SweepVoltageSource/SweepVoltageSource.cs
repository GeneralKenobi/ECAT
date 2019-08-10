using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	public class SweepVoltageSource : ACVoltageSource, ISweepVoltageSource
	{
		#region Constructor

		/// <summary>
		/// 
		/// </summary>
		public SweepVoltageSource()
		{
			OutputValue = 1;
		}

		#endregion
	}
}