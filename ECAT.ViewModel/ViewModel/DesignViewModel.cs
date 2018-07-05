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

		#region Private properties

		/// <summary>
		/// The currently placed wire
		/// </summary>
		private IWire _PlacedWire { get; set; } = null;

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

		#region Public methods

		/// <summary>
		/// Adds a component on the given position
		/// </summary>
		/// <param name="clickPosition"></param>
		public void AddComponent(IPlanePosition clickPosition)
		{
			if (AddingComponents)
			{
				IBaseComponent newComponent = null;
				try
				{
					newComponent = Activator.CreateInstance(DesignManager.GetComponentType(ComponentToAdd)) as IBaseComponent;

					newComponent.Center = clickPosition;
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

		public void SocketClickedHandler(IPartialNode node)
		{
			if(_PlacedWire == null)
			{
				_PlacedWire = DesignManager.ConstructWire();
				DesignManager.CurrentSchematic.AddWire(_PlacedWire);
				_PlacedWire.N1 = node;
			}
			else
			{
				_PlacedWire.N2 = node;
				_PlacedWire = null;
			}
		}

		#endregion
	}
}