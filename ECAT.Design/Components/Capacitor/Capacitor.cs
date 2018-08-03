using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class implementing capacitors, standard implementation of <see cref="ICapacitor"/>
	/// </summary>
	public class Capacitor : TwoTerminal, ICapacitor
    {
		#region Public properties

		/// <summary>
		/// Capacitance of this <see cref="Capacitor"/>
		/// </summary>
		public double Capacitance { get; set; }

		#endregion
	}
}