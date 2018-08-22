namespace ECAT.Core
{
	/// <summary>
	/// Contains information about power
	/// </summary>
	public interface IPowerInformation
    {
		#region Properties

		/// <summary>
		/// Average value
		/// </summary>
		double Average { get; }

		/// <summary>
		/// Determines whether power is dissipated/supplied by the element
		/// </summary>
		PowerType AveragePowerType { get; }

		/// <summary>
		/// Maximum instantenous power loss
		/// </summary>
		double MaximumLost { get; }
		
		/// <summary>
		/// Maximum instantenous power supplied
		/// </summary>
		double MaximumSupplied { get; }

		#endregion
	}
}