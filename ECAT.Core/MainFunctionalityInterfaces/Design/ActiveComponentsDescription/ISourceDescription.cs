namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes used to describe <see cref="IActiveComponent"/>s
	/// </summary>
	public interface ISourceDescription : IComponentDescription
	{
		#region Properties

		/// <summary>
		/// Frequency of the described <see cref="IActiveComponent"/>
		/// </summary>
		double Frequency { get; }

		/// <summary>
		/// Type of the <see cref="IActiveComponent"/>
		/// </summary>
		SourceType SourceType { get; }

		/// <summary>
		/// Output value produced by the source (voltage, current, etc.)
		/// </summary>
		double OutputValue { get; }

		#endregion
	}
}