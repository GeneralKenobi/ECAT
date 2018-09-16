using System.Collections.Generic;
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
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public OpAmp() : base(new string[] { "Output " + IoC.Resolve<QuantityNames>().Voltage, "Output " + IoC.Resolve<QuantityNames>().Current,
			"Differential " + IoC.Resolve<QuantityNames>().Voltage}) { }

		#endregion

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

		/// <summary>
		/// Index used to query <see cref="ISimulationResults"/> for produced current
		/// </summary>
		public int ActiveComponentIndex { get; set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Returns info about output voltage of the op-amp
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> GetOutputVoltageInfo()
		{
			ISignalInformation voltageDrop = IoC.Resolve<ISimulationResults>().GetVoltageDropOrZero(TerminalC.NodeIndex);

			foreach (var item in CIFormat.GetSignalInfo(voltageDrop, IoC.Resolve<QuantityNames>().Voltage, IoC.Resolve<ISIUnits>().VoltageShort))
			{
				yield return item;
			}

			bool supplyExceeded = false;

			// Check if positive supply is exceeded
			if(voltageDrop.Maximum > PositiveSupplyVoltage)
			{
				supplyExceeded = true;
				yield return "Positive supply voltage may be exceeded";
			}

			// Check if negative supply is exceeded
			if (voltageDrop.Minimum < NegativeSupplyVoltage)
			{
				supplyExceeded = true;
				yield return "Negative supply voltage may be exceeded";
			}

			// If any supply was exceeded elaborate on the results
			if(supplyExceeded)
			{
				yield return "Consider running a full cycle simulation for more accurate results";
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns info about the op-amp
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			yield return GetOutputVoltageInfo();
			yield return CIFormat.GetSignalInfo(IoC.Resolve<ISimulationResults>().GetCurrentOrZero(ActiveComponentIndex, false),
				IoC.Resolve<QuantityNames>().Current, IoC.Resolve<ISIUnits>().CurrentShort);
			yield return CIFormat.GetSignalInfo(IoC.Resolve<ISimulationResults>().GetVoltageDropOrZero(
				TerminalA.NodeIndex, TerminalB.NodeIndex), IoC.Resolve<QuantityNames>().Voltage, IoC.Resolve<ISIUnits>().VoltageShort);
		}

		#endregion
	}
}