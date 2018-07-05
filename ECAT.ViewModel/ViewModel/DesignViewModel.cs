using Autofac;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
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

		/// <summary>
		/// True if the user is currently placing a wire
		/// </summary>
		public bool PlacingWire => _PlacedWire != null;

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
				var newComponent = IoC.Container.Resolve<IComponentFactory>().Construct(ComponentToAdd);

				newComponent.Center = clickPosition;
				
				DesignManager.CurrentSchematic.AddComponent(newComponent);				
			}
		}

		/// <summary>
		/// Handles clicks onto sockets
		/// </summary>
		/// <param name="node"></param>
		public void SocketClickedHandler(IPartialNode node)
		{
			if(_PlacedWire == null)
			{
				_PlacedWire = IoC.Container.Resolve<IComponentFactory>().ConstructWire();
				DesignManager.CurrentSchematic.AddWire(_PlacedWire);
				_PlacedWire.N1 = node;
			}
			else
			{
				_PlacedWire.N2 = node;
				_PlacedWire = null;
			}
		}

		/// <summary>
		/// Adds a new point to the currently placed wire
		/// </summary>
		/// <param name="position"></param>
		/// <param name="addAtEnd"></param>
		public void AddPointToPlacedWire(IPlanePosition position, bool addAtEnd = true)
		{
			if(!PlacingWire)
			{
				throw new InvalidOperationException("Can't add a point to the placed wire if there is no wire being placed");
			}

			_PlacedWire.AddPoint(position, addAtEnd);
		}

		#endregion
	}
}