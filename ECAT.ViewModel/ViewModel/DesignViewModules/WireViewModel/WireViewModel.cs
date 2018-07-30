using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Specialized;
using System.Windows.Input;
using Autofac;
using System.Diagnostics;
using System.Numerics;

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
			WireSocketClickedCommand = new RelayParametrizedCommand(WireSocketClicked);
			WireClickedCommand = new RelayParametrizedCommand(WireClicked);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The wire this viewmodel is for
		/// </summary>
		public IWire Wire { get; private set; }

		/// <summary>
		/// Length of the hitbox for sockets and wire stroke
		/// </summary>
		public double HitboxLength => AppViewModel.Singleton.RoundTo;

		#endregion

		#region Commands

		/// <summary>
		/// Removes <see cref="Wire"/> from its schematic
		/// </summary>
		public ICommand RemoveWireCommand { get; }

		/// <summary>
		/// Starts extending the wire from the clicked socket
		/// </summary>
		public ICommand WireSocketClickedCommand { get; }

		/// <summary>
		/// Handles clicks made on a wire
		/// </summary>
		public ICommand WireClickedCommand { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="WireClickedCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void WireClicked(object parameter)
		{
			if (parameter is Complex position)
			{
				AppViewModel.Singleton.DesignVM.DesignManager.WireClickedHandler(Wire,
					IoC.Resolve<IPlanePositionFactory>().Construct(position.Real, position.Imaginary));
			}
		}


		/// <summary>
		/// Method for <see cref="WireSocketClickedCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void WireSocketClicked(object parameter)
		{
			if(parameter is bool b)
			{
				AppViewModel.Singleton.DesignVM.DesignManager.WireSocketClickedHandler(Wire, b);
			}
		}

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