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
	public class ACVoltageSource : TwoTerminalSource, IACVoltageSource
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ACVoltageSource() : base()
		{
			OutputValue = IoC.Resolve<IDefaultValues>().DefaultACVoltageSourceProducedACVoltage;

			// Initialize description values
			_Description.Label = this.Label;
			_Description.Frequency = this.Frequency;
			_Description.SourceType = SourceType.ACVoltageSource;
			_Description.OutputValue = OutputValue;
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="Frequency"/>
		/// </summary>
		private double mFrequency = IoC.Resolve<IDefaultValues>().DefaultACVoltageSourceFrequency;

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		#endregion

		#region Public properties

		/// <summary>
		/// Frequency of the AC voltage produced by this <see cref="ACVoltageSource"/>
		/// </summary>
		public double Frequency
		{
			get => mFrequency;
			set
			{
				// Update the backing store and the description
				mFrequency = value;
				_Description.Frequency = value;
			}
		}

		/// <summary>
		/// Index assigned to this <see cref="IActiveComponent"/>
		/// </summary>
		public int Index { get; set; }

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