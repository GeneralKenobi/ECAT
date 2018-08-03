using ECAT.Core;
using CSharpEnhanced.CoreClasses;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (well almost ideal because the internal admittance can't
	/// be set to infinity - it would not work in the calculations, however it is set to a very big value - 1e100
	/// which corresponds to 1e-100 resistance - practically 0)
	/// </summary>
	public class VoltageSource : TwoTerminal, IVoltageSource
    {
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public VoltageSource()
		{
			Admittance = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;
			ProducedDCVoltage = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

			ProducedCurrent.PropertyChanged += (s, e) => InvokePropertyChanged(nameof(CurrentBA));
		}

		#endregion

		#region Public properties		

		/// <summary>
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage { get; set; }		

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		public RefWrapperPropertyChanged<double> ProducedCurrent { get; set; } = new RefWrapperPropertyChanged<double>();

		/// <summary>
		/// Current flowing from <see cref="TerminalA"/> to <see cref="TerminalB"/> - the opposite of the produced current which
		/// is marked from the positive terminal (<see cref="TerminalB"/>) to the negative terminal (<see cref="TerminalA)"/>
		/// </summary>
		public override double CurrentBA => -ProducedCurrent.Value;

		#endregion
	}
}