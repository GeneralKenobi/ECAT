namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes used to describe <see cref="IActiveComponent"/>s
	/// </summary>
	public interface IActiveComponentDescription
	{
		#region Properties

		/// <summary>
		/// Unique label assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		string Label { get; }

		/// <summary>
		/// Active component index assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		int Index { get; }

		/// <summary>
		/// Frequency of the described <see cref="IActiveComponent"/>
		/// </summary>
		double Frequency { get; }

		/// <summary>
		/// Peak value (voltage or current) produced by the described <see cref="IActiveComponent"/>
		/// </summary>
		double PeakProducedValue { get; }

		/// <summary>
		/// Type of the <see cref="IActiveComponent"/>
		/// </summary>
		ActiveComponentType ComponentType { get; }

		#endregion
	}
}