using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IInductor"/>s
	/// </summary>
	public class InductorEditViewModel : SpecificComponentEditViewModel<IInductor>
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public InductorEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public Properties		

		/// <summary>
		/// Accessor to inductance
		/// </summary>
		public double Inductance
		{
			get => _EditedComponent.Inductance;
			set => _EditedComponent.Inductance = value;
		}

		/// <summary>
		/// Inductance edit field header
		/// </summary>
		public string InductanceEditHeader { get; } = "Inductance [H]";

		#endregion
	}
}