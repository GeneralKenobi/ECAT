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

		/// <summary>
		/// The component that was selected by the user to be added upon clicking on the screen.
		/// If null, then no component is to be added
		/// </summary>
		public IComponentDeclaration ComponentToAdd { get; set; }

		/// <summary>
		/// True if the user is currently adding components
		/// </summary>
		public bool AddingComponents => ComponentToAdd != null;

		#endregion

		#region Commands



		#endregion

		#region Public methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clickPosition"></param>
		public void AddComponent(PlanePosition clickPosition)
		{
			if (AddingComponents)
			{
				IBaseComponent newComponent = null;
				try
				{
					newComponent = Activator.CreateInstance(DesignManager.GetComponentType(ComponentToAdd)) as IBaseComponent;

					newComponent.Handle.Absolute = clickPosition.Absolute;
				}
				catch(Exception e)
				{
					// If something happened and the element could not be created rethrow the exception
					throw;
				}

				if (newComponent != null)
				{
					DesignManager.CurrentSchematic.AddComponent(newComponent);
				}
			}
		}

		#endregion
	}
}