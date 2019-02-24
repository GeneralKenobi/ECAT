using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Viewmodel for editing <see cref="IACVoltageSource"/>
	/// </summary>
	public class ACVoltageSourceEditViewModel : SourceEditViewModel<IACVoltageSource>
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public ACVoltageSourceEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel, "Peak produced AC voltage [V]") { }

		#endregion

		#region Public Properties

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

		#endregion
	}
}