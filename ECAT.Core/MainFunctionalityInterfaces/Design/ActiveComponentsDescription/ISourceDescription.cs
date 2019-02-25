﻿namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes used to describe <see cref="IActiveComponent"/>s
	/// </summary>
	public interface ISourceDescription : IComponentDescription
	{
		#region Properties

		/// <summary>
		/// Frequency of the described source
		/// </summary>
		double Frequency { get; }

		/// <summary>
		/// Type of the described source
		/// </summary>
		SourceType SourceType { get; }

		/// <summary>
		/// Output value produced by the source (voltage, current, etc.)
		/// </summary>
		double OutputValue { get; }

		#endregion
	}
}