using ECAT.Core;
using System;

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

		#region Public properties

		/// <summary>
		/// Accessor to the start sweep frequency
		/// </summary>
		public double StartFrequency
		{
			get => _EditedComponent.StartFrequency;
			set
			{
				_EditedComponent.StartFrequency = value;
				InvokePropertyChanged(nameof(StartFrequency));
			}
		}

		/// <summary>
		/// Start sweep frequency edit field header
		/// </summary>
		public string StartFrequencyHeader { get; } = "Start Frequency [Hz]";

		/// <summary>
		/// Accessor to the end sweep frequency
		/// </summary>
		public double EndFrequency
		{
			get => _EditedComponent.EndFrequency;
			set => _EditedComponent.EndFrequency = value;
		}

		/// <summary>
		/// End sweep frequency edit field header
		/// </summary>
		public string EndFrequencyHeader { get; } = "End Frequency [Hz]";

		#endregion
	}
}