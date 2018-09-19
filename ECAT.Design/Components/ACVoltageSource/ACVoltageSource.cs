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
		protected IEnumerable<string> GetPowerInfoAC(ISignalInformation powerInformation)
		{
			// Minimum instantenous power
			yield return CIFormat.LineInfo("Minimum instantenous " + IoC.Resolve<IQuantityNames>().Power,
				powerInformation.Minimum, IoC.Resolve<ISIUnits>().PowerShort);
			
			// Maximum instantenous power
			yield return CIFormat.LineInfo("Maximum instantenous " + IoC.Resolve<IQuantityNames>().Power,
				powerInformation.Maximum, IoC.Resolve<ISIUnits>().PowerShort);

			// Average power
			yield return CIFormat.LineInfo("Average " + IoC.Resolve<IQuantityNames>().Power,
				powerInformation.Average, IoC.Resolve<ISIUnits>().PowerShort);
		}

		/// <summary>
		/// Returns current info plus power info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			var current = IoC.Resolve<ISimulationResultsProvider>().Value.Current.Get(ActiveComponentIndex, InvertedVoltageCurrentDirections);
			yield return GetCurrentInfo(current);
			yield return GetPowerInfoAC(IoC.Resolve<ISimulationResultsProvider>().Value.Power.Get(this));
		}

		#endregion
	}
}