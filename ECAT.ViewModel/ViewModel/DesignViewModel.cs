using Autofac;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for circuit design
	/// </summary>
    public class DesignViewModel : BaseViewModel, IDisposable
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public DesignViewModel()
		{			
			_DesignManagerScope = IoC.Container.BeginLifetimeScope();

			if(_DesignManagerScope.TryResolve(out IDesignManager manager))
			{
				DesignManager = manager;
			}
			else
			{
				throw new Exception();
			}
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Lifetime scope 
		/// </summary>
		private ILifetimeScope _DesignManagerScope { get; set; }

		#endregion

		#region Public properties

		/// <summary>
		/// Provided implementation of the <see cref="IDesignManager"/> interface
		/// </summary>
		public IDesignManager DesignManager { get; private set; }

		/// <summary>
		/// Disposes of the allocated resources
		/// </summary>
		public void Dispose()
		{
			_DesignManagerScope.Dispose();
			_DesignManagerScope = null;
		}

		#endregion

		#region Commands

		

		#endregion
	}
}