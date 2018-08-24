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
			// Inform that instantenous max/min power is not calculated for bias
			yield return "Minimum/maximum instantenous power is not available in bias";
			yield return "Hint: consider running a full cycle simulation";

			// If average is not a NaN, display it
			if (!double.IsNaN(powerInformation.Average))
			{
				yield return "Average power: " + SIHelpers.ToSIStringExcludingSmallPrefixes(powerInformation.Average, "W", 4);
			}
			// Otherwise inform it couldn't have been calculated
			else
			{
				yield return "Average power cannot be determined in bias method due to multiple AC frequencies";
				yield return "Hint: consider running a full cycle simulation";
			}
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