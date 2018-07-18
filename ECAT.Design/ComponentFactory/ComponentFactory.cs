using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ECAT.Design
{
	/// <summary>
	/// Contains all component types and constructs them as needed
	/// </summary>
	public class ComponentFactory : IComponentFactory
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComponentFactory()
		{
			
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="ImplementedComponents"/>
		/// TODO: When a file system is implemented, move the content to a file and read it from there
		/// </summary>
		private ObservableCollection<IComponentDeclaration> _ImplementedComponents { get; } =
			new ObservableCollection<IComponentDeclaration>()
		{
			new ComponentDeclaration(ComponentIDEnumeration.Resistor, "Resistor", 2, ComponentType.Passive),
			new ComponentDeclaration(ComponentIDEnumeration.VoltageSource, "Voltage Source", 2, ComponentType.Passive),
			new ComponentDeclaration(ComponentIDEnumeration.CurrentSource, "Current Source", 2, ComponentType.Passive),
		};

		/// <summary>
		/// Dictionary of component IDs and types assigned to them
		/// </summary>
		private Dictionary<ComponentIDEnumeration, Type> _ComponentTypesByID { get; } = new Dictionary<ComponentIDEnumeration, Type>()
		{
			{ComponentIDEnumeration.Resistor, typeof(Resistor) },
			{ComponentIDEnumeration.VoltageSource, typeof(VoltageSource) },
			{ComponentIDEnumeration.CurrentSource, typeof(CurrentSource) },
		};

		/// <summary>
		/// Dictionary of component interfaces and types assigned to them
		/// </summary>
		private Dictionary<Type, Type> _ComponentTypesByInterface { get; } = new Dictionary<Type, Type>()
		{
			{typeof(IResistor), typeof(Resistor) },
			{typeof(IVoltageSource), typeof(VoltageSource) },
			{typeof(ICurrentSource), typeof(CurrentSource) },
		};

		#endregion

		#region Public properties

		/// <summary>
		/// Collection of names of all components that are implemented and usable
		/// </summary>
		public ReadOnlyObservableCollection<IComponentDeclaration> ImplementedComponents { get; }

		#endregion

		// TODO: Implement a good way of storing and getting component types, ids and other parameters.

		/// <summary>
		/// Constructs and returns a component based on the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown if the interface does not correspond to a defined component or
		/// the interface is not a final implementation (eg. it's a <see cref="ITwoTerminal"/>)</exception>
		public IBaseComponent Construct<T>() where T : IBaseComponent
		{
			if(_ComponentTypesByInterface.TryGetValue(typeof(T), out var result))
			{
				return result.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as IBaseComponent;
			}

			throw new ArgumentException(nameof(T) + " is not a recognized (or final) component type");
		}

		/// <summary>
		/// Constructs a new <see cref="IBaseComponent"/> based on the given <see cref="IComponentDeclaration"/>
		/// </summary>
		/// <param name="declaration"></param>
		/// <returns></returns>
		public IBaseComponent Construct(IComponentDeclaration declaration) => Construct(declaration.ID);

		/// <summary>
		/// Constructs a new <see cref="IBaseComponent"/> based on the given component id
		/// </summary>
		/// <param name="componentID"></param>
		/// <returns></returns>
		public IBaseComponent Construct(ComponentIDEnumeration componentID)
		{
			if (_ComponentTypesByID.TryGetValue(componentID, out var result))
			{
				return result.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as IBaseComponent;
			}
			
			throw new ArgumentException($"No component matches the ID: {componentID}");
		}

		/// <summary>
		/// Constructs a new <see cref="IWire"/>
		/// </summary>
		/// <returns></returns>
		public IWire ConstructWire() => new Wire();
	}
}