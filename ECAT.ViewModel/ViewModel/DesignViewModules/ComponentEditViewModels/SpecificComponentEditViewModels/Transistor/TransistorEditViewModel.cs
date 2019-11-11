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
	}
}