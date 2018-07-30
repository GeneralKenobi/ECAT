using Autofac;
using ECAT.Core;
using System;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for simulation module
	/// </summary>
	public class SimulationViewModel : BaseViewModel
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public SimulationViewModel()
		{
			if (IoC.Container.TryResolve(out ISimulationManager manager))
			{
				SimulationManager = manager;
			}
			else
			{
				throw new Exception();
			}
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Provided implementation of the <see cref="IDesignManager"/> interface
		/// </summary>
		public ISimulationManager SimulationManager { get; private set; }

		#endregion
	}
}