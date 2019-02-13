using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class implementing AC voltage sources, standard implementation of <see cref="IACVoltageSource"/>
	/// </summary>
	[DisplayVoltageInfo(nameof(TerminalA), nameof(TerminalB), 0, "Voltage drop")]
	[DisplayCurrentInfo(sectionIndex: 1)]
	[DisplayPowerInfo(sectionIndex: 2)]
	public class ACVoltageSource : TwoTerminal, IACVoltageSource
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ACVoltageSource()
		{
			ProducedDCVoltage = IoC.Resolve<IDefaultValues>().DefaultACVoltageSourceDCOffset;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		#endregion

		#region Public properties

		/// <summary>
		/// DC voltage produced by this <see cref="IACVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

		/// <summary>
		/// Index used to query <see cref="ISimulationResults"/> for produced current
		/// </summary>
		public int ActiveComponentIndex { get; set; }

		/// <summary>
		/// Frequency of the AC voltage produced by this <see cref="ACVoltageSource"/>
		/// </summary>
		public double Frequency { get; set; } = IoC.Resolve<IDefaultValues>().DefaultACVoltageSourceFrequency;

		/// <summary>
		/// The positive peak value of the produced voltage sine wave
		/// </summary>
		public double PeakProducedVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultACVoltageSourceProducedACVoltage;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of an <see cref="IACVoltageSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		#endregion
	}
}