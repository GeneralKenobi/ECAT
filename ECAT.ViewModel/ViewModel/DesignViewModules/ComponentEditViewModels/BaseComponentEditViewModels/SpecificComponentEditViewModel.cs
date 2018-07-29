using System;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Base class for view models of specific <see cref="IBaseComponent"/>s, checks if a proper type was passed, exposes the
	/// edited part to inheriting classes
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SpecificComponentEditViewModel<T> : BaseComponentEditViewModel
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public SpecificComponentEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel)
		{
			// Check if the passed view model actually contains an IResistor
			if (componentViewModel.Component is T castedComponent)
			{
				_EditedComponent = castedComponent;
			}
			else
			{
				throw new ArgumentException(nameof(componentViewModel) + " does not contain an of type " + nameof(T));
			}
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// The edited component
		/// </summary>
		protected T _EditedComponent { get; }

		#endregion
	}
}