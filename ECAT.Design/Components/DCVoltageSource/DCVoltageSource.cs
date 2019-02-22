using ECAT.Core;
using System.Numerics;
using System.Collections.Generic;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	[DisplayCurrentInfo(sectionIndex: 1)]
	[DisplayPowerInfo(sectionIndex: 2)]
	public class DCVoltageSource : TwoTerminalSource, IDCVoltageSource
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public DCVoltageSource()
		{
			OutputValue = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

			// Initialize description
			_Description.Label = this.Label;
			_Description.Frequency = 0;
			_Description.SourceType = SourceType.DCVoltageSource;
			_Description.OutputValue = OutputValue;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of an <see cref="IDCVoltageSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		#endregion
	}
}