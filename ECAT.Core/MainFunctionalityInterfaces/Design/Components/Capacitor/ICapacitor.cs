namespace ECAT.Core
{
	/// <summary>
	/// Interface for capacitors
	/// </summary>
	public interface ICapacitor : ITwoTerminal
    {
		#region Properties

		/// <summary>
		/// Capacitance of this <see cref="ICapacitor"/>
		/// </summary>
		double Capacitance { get; set; }

		#endregion
	}
}