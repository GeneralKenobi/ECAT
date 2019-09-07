using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IOpAmp"/>s
	/// </summary>
	public class TransistorEditViewModel<T> : SpecificComponentEditViewModel<T>
		where T : ITransistor
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public TransistorEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public properties
		
		/// <summary>
		/// If true, small-signal model of the transistor will be used
		/// </summary>
		public bool SmallSignalModel
		{
			get => _EditedComponent.SmallSignalModel;
			set => _EditedComponent.SmallSignalModel = value;
		}

		#endregion
	}
}