using System.Collections.Generic;
using CSharpEnhanced.Helpers;
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
			// Present characteristic info, if a value is a NaN inform it's unavailable

			// Minimum instantenous power
			yield return "Minimum instantenous power: " + (double.IsNaN(powerInformation.Minimum) ? "unavailable" :
				SIHelpers.ToSIStringExcludingSmallPrefixes(powerInformation.Minimum, "W", 4));
			
			// Maximum instantenous power
			yield return "Maximum instantenous power: " + (double.IsNaN(powerInformation.Maximum) ? "unavailable" :
				SIHelpers.ToSIStringExcludingSmallPrefixes(powerInformation.Maximum, "W", 4));

			// Average power
			yield return "Average power: " + (double.IsNaN(powerInformation.Average) ? "unavailable" :
				SIHelpers.ToSIStringExcludingSmallPrefixes(powerInformation.Average, "W", 4));
		}

		/// <summary>
		/// Returns current info plus power info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			var current = IoC.Resolve<ISimulationResults>().GetCurrentOrZero(ActiveComponentIndex, InvertedVoltageCurrentDirections);
			yield return GetCurrentInfo(current);
			yield return GetPowerInfoAC(IoC.Resolve<ISimulationResults>().GetPower(current, this));
		}

		#endregion
	}
}