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
    public class DesignViewModel : BaseViewModel
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public DesignViewModel()
		{
			if(IoC.Container.TryResolve(out IDesignManager manager))
			{
				DesignManager = manager;
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
		public IDesignManager DesignManager { get; private set; }

		#endregion

		#region Commands

		

		#endregion
	}
}