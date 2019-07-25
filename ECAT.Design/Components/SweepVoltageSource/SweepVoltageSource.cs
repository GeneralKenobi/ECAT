using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	public class SweepVoltageSource : TwoTerminalSource, ISweepVoltageSource
	{
		#region Public properties

		/// <summary>
		///  Index of this <see cref="IActiveComponent"/>
		/// </summary>
		public int Index { get; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns admittance of this component
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => 0;

		#endregion
	}
}