namespace ECAT.Core
{
	/// <summary>
	/// Interface for Inductors
	/// </summary>
	public interface IInductor : ITwoTerminal
	{
		#region Properties

		/// <summary>
		/// Inductance of this Inductor
		/// </summary>
		double Inductance { get; set; }

		#endregion
	}
}