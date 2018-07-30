using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// View model for edits specific to <see cref="IVoltageSource"/>
	/// </summary>
	public class VoltageSourceEditViewModel : SpecificComponentEditViewModel<IVoltageSource>
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageSourceEditViewModel(ComponentViewModel viewModel) : base(viewModel) { }

		#endregion

		#region Public properties

		/// <summary>
		/// The header for the edit with produced voltage
		/// </summary>
		public string ProducedVoltageEditHeader { get; } = "Produced Voltage [V]";

		/// <summary>
		/// Accessor to the produced voltage on the edited <see cref="IVoltageSource"/>
		/// </summary>
		public double ProducedVoltage
		{
			get => _EditedComponent.ProducedVoltage;
			set => _EditedComponent.ProducedVoltage = value; 
		}

		#endregion
	}
}