using System.Collections.Generic;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class implementing AC voltage sources, standard implementation of <see cref="IACVoltageSource"/>
	/// </summary>
	public class ACVoltageSource : VoltageSource, IACVoltageSource
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

		#region Public properties

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
		/// Returns info related to power
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<string> GetPowerInfoAC(IPowerInformation powerInformation)
		{
			// Minimum instantenous power
			yield return CIFormat.LineInfo("Minimum instantenous " + QuantityNames.Singleton.Power,
				powerInformation.Minimum, SIUnits.Singleton.PowerShort);
			
			// Maximum instantenous power
			yield return CIFormat.LineInfo("Maximum instantenous " + QuantityNames.Singleton.Power,
				powerInformation.Maximum, SIUnits.Singleton.PowerShort);

			// Average power
			yield return CIFormat.LineInfo("Average " + QuantityNames.Singleton.Power,
				powerInformation.Average, SIUnits.Singleton.PowerShort);
		}

		/// <summary>
		/// Returns current info plus power info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			var current = IoC.Resolve<ISimulationResults>().GetCurrentOrZero(ActiveComponentIndex, InvertedVoltageCurrentDirections);
			yield return GetCurrentInfo(current);
			yield return GetPowerInfoAC(IoC.Resolve<ISimulationResults>().GetPower(this));
		}

		#endregion
	}
}