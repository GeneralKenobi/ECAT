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
	public class CurrentSource : TwoTerminalSource, ICurrentSource
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentSource()
		{
			OutputValue = IoC.Resolve<IDefaultValues>().DefaultCurrentSourceProducedCurrent;

			// Initialize description values
			_Description.Label = this.Label;
			_Description.Frequency = 0;
			_Description.SourceType = SourceType.DCCurrentSource;			
			_Description.OutputValue = OutputValue;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Admittance of this <see cref="ICurrentSource"/> (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;

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