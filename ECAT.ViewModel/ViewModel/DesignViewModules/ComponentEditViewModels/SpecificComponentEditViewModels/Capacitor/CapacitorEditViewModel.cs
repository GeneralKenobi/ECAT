using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="ICapacitor"/>s
	/// </summary>
	public class CapacitorEditViewModel : SpecificComponentEditViewModel<ICapacitor>
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public CapacitorEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public Properties		

		/// <summary>
		/// Accessor to capacitance
		/// </summary>
		public double Capacitance
		{
			get => _EditedComponent.Capacitance;
			set => _EditedComponent.Capacitance = value;
		}

		/// <summary>
		/// Capacitance edit field header
		/// </summary>
		public string CapacitanceEditHeader { get; } = "Capacitance [F]";

		#endregion
	}
}