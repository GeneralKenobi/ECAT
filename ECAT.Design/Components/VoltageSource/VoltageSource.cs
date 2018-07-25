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
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public VoltageSource()
		{
			Admittance = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;
			ProducedVoltage = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedVoltage { get; set; }

		/// <summary>
		/// Current through the source, flowing from terminal A to terminal B
		/// </summary>
		public RefWrapperPropertyChanged<double> CurrentBA { get; set; } = new RefWrapperPropertyChanged<double>();

		#endregion
	}
}