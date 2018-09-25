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
	}
}