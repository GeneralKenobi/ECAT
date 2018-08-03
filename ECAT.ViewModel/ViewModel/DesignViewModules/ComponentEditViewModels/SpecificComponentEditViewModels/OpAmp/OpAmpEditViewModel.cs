using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IOpAmp"/>s
	/// </summary>
	public class OpAmpEditViewModel : SpecificComponentEditViewModel<IOpAmp>
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public OpAmpEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public properties

		/// <summary>
		/// Header to display above the <see cref="OpenLoopGain"/> edit control
		/// </summary>
		public string OpenLoopGainEditHeader { get; } = "Open loop gain [V/V]";

		/// <summary>
		/// Header to display above the <see cref="PositiveSupplyVoltage"/> edit control
		/// </summary>
		public string PositiveSupplyVoltageEditHeader { get; } = "Positive supply [V]";

		/// <summary>
		/// Header to display above the <see cref="NegativeSupplyVoltage"/> edit control
		/// </summary>
		public string NegativeSupplyVoltageEditHeader { get; } = "Negative supply [V]";

		/// <summary>
		/// Accessor to the open loop gain of the op-amp
		/// </summary>
		public double OpenLoopGain
		{
			get => _EditedComponent.OpenLoopGain;
			set => _EditedComponent.OpenLoopGain = value;
		}

		/// <summary>
		/// Accessor to the positive supply voltage of the op-amp
		/// </summary>
		public double PositiveSupplyVoltage
		{
			get => _EditedComponent.PositiveSupplyVoltage;
			set => _EditedComponent.PositiveSupplyVoltage = value;
		}

		/// <summary>
		/// Accessor to the negative supply voltage of the op-amp
		/// </summary>
		public double NegativeSupplyVoltage
		{
			get => _EditedComponent.NegativeSupplyVoltage;
			set => _EditedComponent.NegativeSupplyVoltage = value;
		}

		#endregion
	}
}