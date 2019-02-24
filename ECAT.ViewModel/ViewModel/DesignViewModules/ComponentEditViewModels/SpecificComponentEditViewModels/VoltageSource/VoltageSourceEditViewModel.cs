using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// View model for edits specific to <see cref="IVoltageSource"/>
	/// </summary>
	public class VoltageSourceEditViewModel : SourceEditViewModel<IDCVoltageSource>
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageSourceEditViewModel(ComponentViewModel viewModel) : base(viewModel, "Produced DC Voltage [V]") { }

		#endregion
	}
}