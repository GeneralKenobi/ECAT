using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class representing an operational amplifier, standard implementation of <see cref="IOpAmp"/>.
	/// <see cref="ThreeTerminal.TerminalA"/> is the non-inverting input, <see cref="ThreeTerminal.TerminalB"/>
	/// is the inverting input and <see cref="ThreeTerminal.TerminalC"/> is the output.
	/// </summary>
	public class OpAmp : ThreeTerminal, IOpAmp
	{
		#region Protected properties

		/// <summary>
		/// Shift relative to center applied to terminal A (non-inverting input)
		/// </summary>
		protected override Complex _TerminalAShift { get; } = new Complex(-150, 50);

		/// <summary>
		/// Shift relative to center applied to terminal B (inverting input)
		/// </summary>
		protected override Complex _TerminalBShift { get; } = new Complex(-150, -50);

		/// <summary>
		/// Shift relative to center applied to terminal C (output)
		/// </summary>
		protected override Complex _TerminalCShift { get; } = new Complex(150, 0);

		#endregion

		#region Public properties

		/// <summary>		
		/// Positive supply voltage - output cannot be greater than this value
		/// </summary>
		public double PositiveSupplyVoltage { get; set; } = 15;

		/// <summary>
		/// Negative supply voltage - output cannot be smaller than this value
		/// </summary>
		public double NegativeSupplyVoltage { get; set; } = -15;

		/// <summary>
		/// Open loop gain - voltage gain defined as output voltage divided by differential voltage (U+ - U-)
		/// </summary>
		public double OpenLoopGain { get; set; } = 1e6;

		/// <summary>
		/// Width of the <see cref="OpAmp"/> in horizontal position
		/// </summary>
		public override double Width { get; } = 300;

		/// <summary>
		/// Height of the <see cref="OpAmp"/> in horizontal position
		/// </summary>
		public override double Height { get; } = 200;

		#endregion
	}
}