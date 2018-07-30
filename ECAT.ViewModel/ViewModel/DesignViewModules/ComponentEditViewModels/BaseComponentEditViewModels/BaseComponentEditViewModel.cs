using ECAT.Core;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Base view model for viewmodels that are used to edit <see cref="IBaseComponent"/>s
	/// </summary>
	public class BaseComponentEditViewModel : BaseViewModel
    {
		#region Constructor

		/// <summary>
		/// Default constructor which takes as an argument the view model of the component to edit
		/// </summary>
		/// <param name="componentViewModel"></param>
		public BaseComponentEditViewModel(ComponentViewModel componentViewModel)
		{
			_EditedComponentViewModel = componentViewModel;
			EditedComponentDeclaration = IoC.Resolve<IComponentFactory>().GetDeclaration<IResistor>();
		}

		#endregion

		#region Private properties

		/// <summary>
		/// ViewModel of the component to edit
		/// </summary>
		private ComponentViewModel _EditedComponentViewModel { get; }

		/// <summary>
		/// Component that is represented by this ViewModel
		/// </summary>
		private IBaseComponent _EditedComponent => _EditedComponentViewModel.Component;

		#endregion

		#region Public Properties

		/// <summary>
		/// The declaration of the currently edited component
		/// </summary>
		public IComponentDeclaration EditedComponentDeclaration { get; }

		#endregion

		#region Commands

		/// <summary>
		/// Getter to the rotate left command from the edited component
		/// </summary>
		public ICommand RotateLeftCommand => _EditedComponentViewModel.RotateLeftCommand;

		/// <summary>
		/// Getter to the rotate right command from the edited component
		/// </summary>
		public ICommand RotateRightCommand => _EditedComponentViewModel.RotateRightCommand;

		/// <summary>
		/// Removes the currently edited component from the list of component and consequently deletes it
		/// </summary>
		public ICommand RemoveComponentCommand => _EditedComponentViewModel.RemoveComponentCommand;

		#endregion

		#region Public methods

		/// <summary>
		/// Returns true if the edited component is <paramref name="component"/>
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public bool IsComponentEdited(IBaseComponent component) => _EditedComponent == component;

		#endregion
	}
}