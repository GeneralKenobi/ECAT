using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all two-terminal components
	/// </summary>
	public abstract class TwoTerminal : BaseComponent, ITwoTerminal
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected TwoTerminal() : this(Enumerable.Empty<string>()) { }

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected TwoTerminal(IEnumerable<string> headers) : base(headers)
		{
			TerminalA = new Terminal(new PlanePosition(Complex.Zero, _TerminalAShift));
			TerminalB = new Terminal(new PlanePosition(Complex.Zero, _TerminalBShift));
			IoC.Resolve<ISimulationManager>().SimulationCompleted += (s, e) => InvokePropertyChanged(nameof(InvertedVoltageCurrentDirections));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// The shift assigned to <see cref="TerminalA"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalAShift => new Complex(-Width / 2, 0);

		/// <summary>
		/// The shift assigned to <see cref="TerminalB"/>, override to provide custom value
		/// </summary>
		protected virtual Complex _TerminalBShift => new Complex(Width / 2, 0);

		/// <summary>
		/// Gets the voltage drop between nodes B and A (with A being the reference node)
		/// </summary>
		protected ISignalInformation _VoltageDrop => ChangeVIDirections ?
			IoC.Resolve<ISimulationResults>().GetVoltageDropOrZero(TerminalB.NodeIndex, TerminalA.NodeIndex) :
			IoC.Resolve<ISimulationResults>().GetVoltageDropOrZero(TerminalA.NodeIndex, TerminalB.NodeIndex);

		#endregion

		#region Public properties		

		/// <summary>
		/// Width of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Width => 200;

		/// <summary>
		/// Height of the control in circuit design in the default, horizontal position
		/// </summary>
		public override double Height => 100;

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public ITerminal TerminalA { get; }

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		public ITerminal TerminalB { get; }

		/// <summary>
		/// True if the standard voltage drop direciton (Vb - Va) was inverted
		/// </summary>
		public virtual bool InvertedVoltageCurrentDirections => _VoltageDrop.InvertedDirection;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns info related to voltage
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<string> GetVoltageInfo(ISignalInformation voltageDrop) => 
			CIFormat.GetSignalInfo(voltageDrop, QuantityNames.Singleton.Voltage, SIUnits.Singleton.VoltageShort);		

		/// <summary>
		/// Returns info related to current
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<string> GetCurrentInfo(ISignalInformation currentInfo) =>
			CIFormat.GetSignalInfo(currentInfo, QuantityNames.Singleton.Current, SIUnits.Singleton.CurrentShort);		

		/// <summary>
		/// Assigns positions to all <see cref="ITerminal"/>s
		/// </summary>
		protected override void UpdateAbsoluteTerminalPositions()
		{
			TerminalA.Position.Absolute = new Complex(Center.X, Center.Y);
			TerminalB.Position.Absolute = new Complex(Center.X, Center.Y);
		}

		/// <summary>
		/// Rotates all partial nodes by <paramref name="degrees"/>
		/// </summary>
		/// <param name="degrees"></param>
		protected override void RotateTerminals(double degrees)
		{
			TerminalA.Position.RotationAngle += degrees;
			TerminalB.Position.RotationAngle += degrees;
		}

		/// <summary>
		/// Returns admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>, called by <see cref="GetAdmittance(double)"/>
		/// </summary>
		protected abstract Complex CalculateAdmittance(double frequency);

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a list with all terminals in this component
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ITerminal> GetTerminals()	
		{
			yield return TerminalA;
			yield return TerminalB;
		}

		/// <summary>
		/// Returns admittance between <see cref="TerminalA"/> and <see cref="TerminalB"/>
		/// </summary>
		public Complex GetAdmittance(double frequency)
		{
			// Check if the frequency is in the allowed range (nonnegative)
			if(frequency < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(frequency) + " cannot be negative");
			}

			// Call the helper method
			return CalculateAdmittance(frequency);
		}

		/// <summary>
		/// Returns conductance between <see cref="TerminalA"/> and <see cref="TerminalB"/> for DC signals. Conductance is the real part
		/// of admittance. For DC all elements have admittance with only real component being non-zero so this method returns the same
		/// as <see cref="GetAdmittance(double)"/> for 0 frequency.
		/// </summary>
		/// <returns></returns>
		public double GetConductance() => GetAdmittance(0).Real;

		#endregion
	}
}