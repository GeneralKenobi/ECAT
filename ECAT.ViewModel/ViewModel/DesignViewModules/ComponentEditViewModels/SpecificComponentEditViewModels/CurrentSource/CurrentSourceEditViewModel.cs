using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// View model for edits specific to <see cref="ICurrentSource"/>
	/// </summary>
	public class CurrentSourceEditViewModel : SpecificComponentEditViewModel<ICurrentSource>
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentSourceEditViewModel(ComponentViewModel viewModel) : base(viewModel) { }

		#endregion

		#region Public properties

		/// <summary>
		/// The header for the edit with produced voltage
		/// </summary>
		public string ProducedCurrentEditHeader { get; } = "Produced Current [A]";

		/// <summary>
		/// Accessor to the produced voltage on the edited <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedCurrent
		{
			get => _EditedComponent.ProducedCurrent;
			set => _EditedComponent.ProducedCurrent = value; 
		}

		#endregion
	}
}