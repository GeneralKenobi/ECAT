using System.Collections.Generic;
using System.Numerics;
using CSharpEnhanced.Helpers;
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
		public double PositiveSupplyVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultOpAmpPositiveSupplyVoltage;

		/// <summary>
		/// Negative supply voltage - output cannot be smaller than this value
		/// </summary>
		public double NegativeSupplyVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultOpAmpNegativeSupplyVoltage;

		/// <summary>
		/// Open loop gain - voltage gain defined as output voltage divided by differential voltage (U+ - U-)
		/// </summary>
		public double OpenLoopGain { get; set; } = IoC.Resolve<IDefaultValues>().DefaultOpAmpOpenLoopGain;

		/// <summary>
		/// Width of the <see cref="OpAmp"/> in horizontal position
		/// </summary>
		public override double Width { get; } = 300;

		/// <summary>
		/// Height of the <see cref="OpAmp"/> in horizontal position
		/// </summary>
		public override double Height { get; } = 200;

		#endregion

		#region Public methods

		/// <summary>
		/// Returns the info that is to be presented, for example, on pointer over. It should include voltage drop(s) across the element,
		/// current(s) through the element, etc.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetComponentInfo()
		{
			yield return string.Empty;
			//yield return "Output voltage: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
			//	(TerminalC.Potentials != null ? TerminalC.Potentials.Value : 0), "V", imaginaryAsJ: true);
			//yield return "Non-inverting input potential: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
			//	(TerminalA.Potentials != null ? TerminalA.Potentials.Value : 0), "V", imaginaryAsJ: true);
			//yield return "Inverting input potential: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
			//	(TerminalB.Potentials != null ? TerminalB.Potentials.Value : 0), "V", imaginaryAsJ: true);
			//yield return "Differential voltage: " + SIHelpers.ToAltSIStringExcludingSmallPrefixes(
			//	(TerminalA.Potentials != null && TerminalB.Potentials != null ? TerminalA.Potentials.Value - TerminalB.Potentials.Value : 0),
			//	"V", imaginaryAsJ: true);
		}

		#endregion
	}
}