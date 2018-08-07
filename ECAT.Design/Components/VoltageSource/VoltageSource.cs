using ECAT.Core;
using CSharpEnhanced.CoreClasses;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public class VoltageSource : TwoTerminal, IVoltageSource
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public VoltageSource()
		{
			ProducedCurrent.PropertyChanged += (s, e) => InvokePropertyChanged(nameof(CurrentBA));
		}

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		#endregion

		#region Public properties		

		/// <summary>
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		public RefWrapperPropertyChanged<Complex> ProducedCurrent { get; set; } = new RefWrapperPropertyChanged<Complex>();

		/// <summary>
		/// Current flowing from <see cref="TerminalA"/> to <see cref="TerminalB"/> - the opposite of the produced current which
		/// is marked from the positive terminal (<see cref="TerminalB"/>) to the negative terminal (<see cref="TerminalA)"/>
		/// </summary>
		public override Complex CurrentBA => -ProducedCurrent.Value;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of an <see cref="IVoltageSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		#endregion
	}
}