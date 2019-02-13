using ECAT.Core;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Current source is an ideal source that can supply a chosen value of current to an arbitrary load
	/// (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	[DisplayVoltageInfo(nameof(TerminalA), nameof(TerminalB), 0, "Voltage")]
	[DisplayPowerInfo(sectionIndex:1)]
	public class CurrentSource : TwoTerminal, ICurrentSource
	{
		#region Private properties

		/// <summary>
		/// Admittance of this <see cref="ICurrentSource"/> (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;

		#endregion

		#region Public properties

		/// <summary>
		/// Accessor to the current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedCurrent { get; set; } = IoC.Resolve<IDefaultValues>().DefaultCurrentSourceProducedCurrent;

		#endregion

		#region Public methods

		/// <summary>
		/// Returns the admittance of this <see cref="ICurrentSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		#endregion
	}
}