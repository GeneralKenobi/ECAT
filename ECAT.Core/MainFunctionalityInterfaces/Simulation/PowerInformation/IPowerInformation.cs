namespace ECAT.Core
{
	/// <summary>
	/// Contains information about power
	/// </summary>
	public interface IPowerInformation
    {
		#region Properties

		/// <summary>
		/// Average value, negative power is supplied, positive power is dissipated
		/// </summary>
		double Average { get; }

		/// <summary>
		/// Determines whether power is dissipated/supplied by the element. Negative power is supplied, positive power is dissipated		
		/// </summary>
		PowerType AveragePowerType { get; }

		/// <summary>
		/// Maximum instantenous power value
		/// </summary>
		double Maximum { get; }

		/// <summary>
		/// Minimum instantenous power value 
		/// </summary>
		double Minimum { get; }

		#endregion
	}
}