using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Class representing an operational amplifier, standard implementation of <see cref="IOpAmp"/>.
	/// <see cref="ThreeTerminal.TerminalA"/> is the non-inverting input, <see cref="ThreeTerminal.TerminalB"/>
	/// is the inverting input and <see cref="ThreeTerminal.TerminalC"/> is the output.
	/// </summary>
	[DisplayVoltageInfo(nameof(TerminalC), 0, "Output Voltage")]
	[DisplayVoltageInfo(nameof(TerminalA), nameof(TerminalB), 1, "Differential Voltage")]
	[DisplayCurrentInfo(sectionIndex: 2)]
	public class OpAmp : ThreeTerminal, IOpAmp
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public OpAmp()
		{
			// Create a description
			_Description = new OpAmpDescription()
			{
				Label = this.Label,
				PositiveSupplyVoltage = this.PositiveSupplyVoltage,
				NegativeSupplyVoltage = this.NegativeSupplyVoltage,
				OpenLoopGain = this.OpenLoopGain,
			};
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="PositiveSupplyVoltage"/>
		/// </summary>
		private double mPositiveSupplyVoltage = IoC.Resolve<IDefaultValues>().DefaultOpAmpPositiveSupplyVoltage;

		/// <summary>
		/// Backing store for <see cref="NegativeSupplyVoltage"/>
		/// </summary>
		private double mNegativeSupplyVoltage = IoC.Resolve<IDefaultValues>().DefaultOpAmpNegativeSupplyVoltage;

		/// <summary>
		/// Backing store for <see cref="OpenLoopGain"/>
		/// </summary>
		private double mOpenLoopGain = IoC.Resolve<IDefaultValues>().DefaultOpAmpOpenLoopGain;

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="Description"/>
		/// </summary>
		private OpAmpDescription _Description { get; }

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
		public double PositiveSupplyVoltage
		{
			get => mPositiveSupplyVoltage;
			set
			{
				mPositiveSupplyVoltage = value;
				_Description.PositiveSupplyVoltage = value;
			}
		}

		/// <summary>
		/// Negative supply voltage - output cannot be smaller than this value
		/// </summary>
		public double NegativeSupplyVoltage
		{
			get => mNegativeSupplyVoltage;
			set
			{
				mNegativeSupplyVoltage = value;
				_Description.NegativeSupplyVoltage = value;
			}
		}

		/// <summary>
		/// Open loop gain - voltage gain defined as output voltage divided by differential voltage (U+ - U-)
		/// </summary>
		public double OpenLoopGain
		{
			get => mOpenLoopGain;
			set
			{
				mOpenLoopGain = value;
				_Description.OpenLoopGain = value;
			}
		}

		/// <summary>
		/// Width of the <see cref="OpAmp"/> in horizontal position
		/// </summary>
		public override double Width { get; } = 300;

		/// <summary>
		/// Height of the <see cref="OpAmp"/> in horizontal position
		/// </summary>
		public override double Height { get; } = 200;

		/// <summary>
		/// Description of this <see cref="IOpAmp"/>
		/// </summary>
		public IOpAmpDescription Description => _Description;

		#endregion
	}
}