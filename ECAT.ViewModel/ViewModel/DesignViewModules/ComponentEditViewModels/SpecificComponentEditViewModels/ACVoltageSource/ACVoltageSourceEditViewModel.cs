using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Viewmodel for editing <see cref="IACVoltageSource"/>
	/// </summary>
	public class ACVoltageSourceEditViewModel : SpecificComponentEditViewModel<IACVoltageSource>
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public ACVoltageSourceEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public Properties

		/// <summary>
		/// Accessor to the peak produced AC voltage
		/// </summary>
		public double PeakProducedVoltage
		{
			get => _EditedComponent.PeakProducedVoltage;
			set => _EditedComponent.PeakProducedVoltage = value;
		}

		/// <summary>
		/// Capacitance edit field header
		/// </summary>
		public string PeakProducedVoltageEditHeader { get; } = "Peak AC voltage [V]";

		/// <summary>
		/// Accessor to the frequency of the produced AC voltage
		/// </summary>
		public double Frequency
		{
			get => _EditedComponent.Frequency;
			set => _EditedComponent.Frequency = value;
		}

		/// <summary>
		/// Capacitance edit field header
		/// </summary>
		public string FrequencyEditHeader { get; } = "Frequency [Hz]";

		/// <summary>
		/// Accessor to the DC offset
		/// </summary>
		public double DCOffset
		{
			get => _EditedComponent.ProducedDCVoltage;
			set => _EditedComponent.ProducedDCVoltage = value;
		}

		/// <summary>
		/// Capacitance edit field header
		/// </summary>
		public string DCOffsetEditHeader { get; } = "DC Offset [V]";

		#endregion
	}
}