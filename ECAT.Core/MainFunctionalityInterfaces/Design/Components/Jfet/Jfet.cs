namespace ECAT.Core
{
	/// <summary>
	/// Interface for Junction Field Effect Transistors. Terminal A maps to gate, terminal B maps to drain and terminal C maps to emitter.
	/// </summary>
	public interface IJfet : ITransistor
	{
		#region Properties

		/// <summary>
		/// Gate
		/// </summary>
		double RGS { get; set; }

		/// <summary>
		/// Small-signal output resistance
		/// </summary>
		double RDS { get; set; }

		/// <summary>
		/// Transconductance
		/// </summary>
		double GM { get; set; }

		#endregion
	}
}