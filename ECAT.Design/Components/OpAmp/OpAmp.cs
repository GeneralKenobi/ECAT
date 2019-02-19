﻿using System.Numerics;
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
			_Description = new SourceDescription()
			{
				Label = this.Label,
				Index = ActiveComponentIndex,
				Frequency = 0,
				ComponentType = SourceType.OpAmp,
			};
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="ActiveComponentIndex"/>
		/// </summary>
		private int mActiveComponentIndex;

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="Description"/>
		/// </summary>
		private SourceDescription _Description { get; }

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
		public int ActiveComponentIndex
		{
			get => mActiveComponentIndex;
			set
			{
				// Update the backing store and value in description
				mActiveComponentIndex = value;
				_Description.Index = value;
			}
		}

		/// <summary>
		/// Description of this <see cref="IActiveComponent"/>
		/// </summary>
		public ISourceDescription Description => _Description;

		#endregion
	}
}