using ECAT.Core;
using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Maths;

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

			ProducedDCVoltageVar = _ProducedVoltageVarSource.Variable;

			ProducedCurrent.PropertyChanged += (s, e) => InvokePropertyChanged(nameof(CurrentBA));
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Source of <see cref="ProducedDCVoltageVar"/> and a backing store for produced current of this <see cref="IVoltageSource"/>
		/// </summary>
		protected Variable.VariableSource _ProducedVoltageVarSource { get; } = new Variable.VariableSource();

		#endregion

		#region Public properties

		/// <summary>
		/// Voltage supplied by this <see cref="IVoltageSource"/>
		/// </summary>
		public Variable ProducedDCVoltageVar { get; }

		/// <summary>
		/// Accessor to the voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage
		{
			get => _ProducedVoltageVarSource.Value.Real;
			set => _ProducedVoltageVarSource.Value = value;
		}

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