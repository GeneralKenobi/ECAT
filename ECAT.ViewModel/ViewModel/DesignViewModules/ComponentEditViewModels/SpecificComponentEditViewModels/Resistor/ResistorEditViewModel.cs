using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IResistor"/>s
	/// </summary>
	public class ResistorEditViewModel : SpecificComponentEditViewModel<IResistor>
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public ResistorEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public Properties		

		/// <summary>
		/// Accessor to resistance
		/// </summary>
		public double Resistance
		{
			get => _EditedComponent.Resistance;
			set => _EditedComponent.Resistance = value;
		}

		/// <summary>
		/// Resistance edit field header
		/// </summary>
		public string ResistanceEditHeader { get; } = "Resistance [Ω]";		

		#endregion
	}
}