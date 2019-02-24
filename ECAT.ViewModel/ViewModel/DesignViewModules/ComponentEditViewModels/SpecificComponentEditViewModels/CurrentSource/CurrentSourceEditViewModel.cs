using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// View model for edits specific to <see cref="ICurrentSource"/>
	/// </summary>
	public class CurrentSourceEditViewModel : SourceEditViewModel<ICurrentSource>
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentSourceEditViewModel(ComponentViewModel viewModel) : base(viewModel, "Produced current [A]") { }

		#endregion
	}
}