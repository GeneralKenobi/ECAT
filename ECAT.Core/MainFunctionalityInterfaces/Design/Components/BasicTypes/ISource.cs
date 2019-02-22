namespace ECAT.Core
{
	/// <summary>
	/// Interface for all sources
	/// </summary>
	public interface ISource
	{
		#region Properties

		/// <summary>
		/// Description of this <see cref="ISource"/>
		/// </summary>
		ISourceDescription Description { get; }

		/// <summary>
		/// Output value of the source (voltage, current, etc.)
		/// </summary>
		double OutputValue { get; set; }

		#endregion
	}
}