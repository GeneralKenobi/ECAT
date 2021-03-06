﻿using ECAT.Core;

namespace ECAT.Simulation
{
	partial class AdmittanceMatrixFactory
	{
		/// <summary>
		/// ISourceDescription for results generated by saturated op-amps
		/// </summary>
		private class OpAmpSaturationSourceDescription : ISourceDescription
		{
			#region Public properties

			/// <summary>
			/// Frequency of the described source
			/// </summary>
			public double Frequency { get; } = 0;

			/// <summary>
			/// Type of the described source
			/// </summary>
			public SourceType SourceType { get; } = SourceType.DCVoltageSource;

			/// <summary>
			/// Frequency category to which this source belongs
			/// </summary>
			public FrequencyCategory FrequencyCategory { get; } = FrequencyCategory.DC;

			/// <summary>
			/// Output value produced by the source (voltage, current, etc.)
			/// </summary>
			public double OutputValue { get; } = 0;

			/// <summary>
			/// Unique label assigned to the <see cref="IBaseComponent"/> that is described by this instance
			/// </summary>
			public IIDLabel Label { get; } = IoC.Resolve<IIDLabel>();

			#endregion
		}
	}
}