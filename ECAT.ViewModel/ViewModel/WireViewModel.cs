using CSharpEnhanced.ICommands;
using ECAT.Core;
using System;
using System.Collections.Specialized;
using System.Windows.Input;
using Autofac;

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

			(Wire.DefiningPoints as INotifyCollectionChanged).CollectionChanged += OnWireDefiningPointsChanged;			
		}

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

		#endregion

		#region Public properties

		/// <summary>
		/// The wire this viewmodel is for
		/// </summary>
		public IWire Wire { get; private set; }

		/// <summary>
		/// Point that the wire begins on
		/// </summary>
		public IPlanePosition WireBeginning => Wire != null && Wire.DefiningPoints.Count > 0 ?
			Wire.DefiningPoints[0] : IoC.Container.Resolve<IPlanePositionFactory>().Construct();

		/// <summary>
		/// Point the wire ends on
		/// </summary>
		public IPlanePosition WireEnding => Wire != null && Wire.DefiningPoints.Count > 0 ?
			Wire.DefiningPoints[Wire.DefiningPoints.Count - 1] : IoC.Container.Resolve<IPlanePositionFactory>().Construct();

		#endregion

		#region Private methods

		/// <summary>
		/// Method for <see cref="WireSocketClickedCommand"/>
		/// </summary>
		/// <param name="parameter"></param>
		private void WireSocketClicked(object parameter)
		{
			if(parameter is bool b)
			{
				AppViewModel.Singleton.DesignVM.WireSocketClickedHandler(Wire, b);
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

		/// <summary>
		/// Method invoked when DefiningPoints of the Wire change; invkes PropertyChanged event for <see cref="WireBeginning"/> and
		/// <see cref="WireEnding"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnWireDefiningPointsChanged(object sender, NotifyCollectionChangedEventArgs e) =>
			InvokePropertyChanged(nameof(WireEnding), nameof(WireBeginning));


		#endregion
	}
}