using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for <see cref="IWire"/>
	/// </summary>
	public class WireViewModel : BaseViewModel
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public WireViewModel(IWire wire)
		{
			Wire = wire ?? throw new ArgumentNullException(nameof(wire));

			RemoveWireCommand = new RelayCommand(RemoveWire);
		}

		#endregion

		#region Commands

		/// <summary>
		/// Removes <see cref="Wire"/> from its schematic
		/// </summary>
		public ICommand RemoveWireCommand { get; set; }

		#endregion

		#region Public properties

		/// <summary>
		/// The wire this viewmodel is for
		/// </summary>
		public IWire Wire { get; private set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="RemoveWireCommand"/>
		/// </summary>
		private void RemoveWire()
		{
			AppViewModel.Singleton.DesignVM.DesignManager.RemoveWire(Wire);
			Wire = null;
		}

		#endregion
	}
}