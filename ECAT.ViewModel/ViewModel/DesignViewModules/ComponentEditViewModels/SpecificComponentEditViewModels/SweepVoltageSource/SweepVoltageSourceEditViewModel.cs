using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// View model for edits specific to <see cref="IVoltageSource"/>
	/// </summary>
	public class SweepVoltageSourceEditViewModel : SpecificComponentEditViewModel<ISweepVoltageSource>
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SweepVoltageSourceEditViewModel(ComponentViewModel viewModel) : base(viewModel) { }

		#endregion
	}
}