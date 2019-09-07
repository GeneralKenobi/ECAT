using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ECAT.Design
{
	/// <summary>
	/// Contains all component types and constructs them as needed
	/// </summary>
	[RegisterAsInstance(typeof(IComponentFactory))]
	public class ComponentFactory : IComponentFactory
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComponentFactory()
		{
			// Construct the dictionary from the existing collections
			var interfacesWithDeclarations = _AssociatedInterfaces.ToDictionary((entry) => entry.Value,
				(entry) => _ImplementedComponents.Find((declaration) => declaration.ID == entry.Key));

			_AssociatedDeclarations = _AssociatedTypes.Select((entry) => new KeyValuePair<Type, IComponentDeclaration>(entry.Value,
			  interfacesWithDeclarations[entry.Key])).Concat(interfacesWithDeclarations).ToDictionary((x) => x.Key, (x) => x.Value);
		}

		#endregion

		#region Private properties

		/// <summary>
		/// List of all implemented components with their declared information. TODO: Read it from a file
		/// </summary>
		private List<IComponentDeclaration> _ImplementedComponents { get; } = new List<IComponentDeclaration>()
		{
			new ComponentDeclaration(ComponentIDEnumeration.Resistor, "Resistor", 2, ComponentType.Passive, ComponentCategory.Impedance),
			new ComponentDeclaration(ComponentIDEnumeration.VoltageSource, "Voltage Source", 2, ComponentType.Passive, ComponentCategory.Source),
			new ComponentDeclaration(ComponentIDEnumeration.CurrentSource, "Current Source", 2, ComponentType.Passive, ComponentCategory.Source),
			new ComponentDeclaration(ComponentIDEnumeration.Ground, "Ground", 1, ComponentType.Passive, ComponentCategory.Other),
			new ComponentDeclaration(ComponentIDEnumeration.OpAmp, "Operational Amplifier", 3, ComponentType.Active, ComponentCategory.ThreeTerminal),
			new ComponentDeclaration(ComponentIDEnumeration.ACVoltageSource, "AC Voltage Source", 2, ComponentType.Passive, ComponentCategory.Source),
			new ComponentDeclaration(ComponentIDEnumeration.Capacitor, "Capacitor", 2, ComponentType.Passive, ComponentCategory.Impedance),
			new ComponentDeclaration(ComponentIDEnumeration.SweepVoltageSource, "Sweep Voltage Source", 2, ComponentType.Active, ComponentCategory.Source),
			new ComponentDeclaration(ComponentIDEnumeration.Voltmeter, "Voltmeter", 2, ComponentType.Passive, ComponentCategory.Other),
			new ComponentDeclaration(ComponentIDEnumeration.Inductor, "Inductor", 2, ComponentType.Passive, ComponentCategory.Impedance),
			new ComponentDeclaration(ComponentIDEnumeration.NpnBjt, "NPN BJT", 2, ComponentType.Active, ComponentCategory.ThreeTerminal),
			new ComponentDeclaration(ComponentIDEnumeration.NChannelJfet, "N-Channel JFET", 2, ComponentType.Active, ComponentCategory.ThreeTerminal),
		};

		/// <summary>
		/// List of types associated with given <see cref="ComponentIDEnumeration"/>. First type is the associated interface,
		/// second type is the implementation type
		/// </summary>
		private Dictionary<ComponentIDEnumeration, Type> _AssociatedInterfaces { get; } = new Dictionary<ComponentIDEnumeration, Type>()
		{
			{ ComponentIDEnumeration.Resistor, typeof(IResistor) },
			{ ComponentIDEnumeration.VoltageSource, typeof(IDCVoltageSource) },
			{ ComponentIDEnumeration.CurrentSource, typeof(ICurrentSource) },
			{ ComponentIDEnumeration.Ground, typeof(IGround) },
			{ ComponentIDEnumeration.OpAmp, typeof(IOpAmp) },
			{ ComponentIDEnumeration.ACVoltageSource, typeof(IACVoltageSource) },
			{ ComponentIDEnumeration.Capacitor, typeof(ICapacitor) },
			{ ComponentIDEnumeration.SweepVoltageSource, typeof(ISweepVoltageSource) },
			{ ComponentIDEnumeration.Voltmeter, typeof(IVoltmeter) },
			{ ComponentIDEnumeration.Inductor, typeof(IInductor) },
			{ ComponentIDEnumeration.NpnBjt, typeof(INpnBjt) },
			{ ComponentIDEnumeration.NChannelJfet, typeof(INChannelJfet) },
		};

		/// <summary>
		/// List of <see cref="ComponentIDEnumeration"/> associated with given types. Constructed based on
		/// <see cref="_AssociatedInterfaces"/> and <see cref="_ImplementedComponents"/>
		/// </summary>
		private Dictionary<Type, IComponentDeclaration> _AssociatedDeclarations { get; }

		/// <summary>
		/// List of types associated with given <see cref="ComponentIDEnumeration"/>. First type is the associated interface,
		/// second type is the implementation type
		/// </summary>
		private Dictionary<Type, Type> _AssociatedTypes { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(IResistor), typeof(Resistor) },
			{ typeof(IDCVoltageSource), typeof(DCVoltageSource) },
			{ typeof(ICurrentSource), typeof(CurrentSource) },
			{ typeof(IGround), typeof(Ground) },
			{ typeof(IOpAmp), typeof(OpAmp) },
			{ typeof(IACVoltageSource), typeof(ACVoltageSource) },
			{ typeof(ICapacitor), typeof(Capacitor) },
			{ typeof(ISweepVoltageSource), typeof(SweepVoltageSource) },
			{ typeof(IVoltmeter), typeof(Voltmeter) },
			{ typeof(IInductor), typeof(Inductor) },
			{ typeof(INpnBjt), typeof(NpnBjt) },
			{ typeof(INChannelJfet), typeof(NChannelJfet) },
		};

		#endregion

		#region Public properties

		/// <summary>
		/// Collection of names of all components that are implemented and usable
		/// </summary>
		public IEnumerable<IComponentDeclaration> ImplementedComponents => _ImplementedComponents;

		#endregion

		#region Public methods

		/// <summary>
		/// Returns declaration of a component based on its ID
		/// </summary>
		/// <param name="componentID"></param>
		/// <returns></returns>
		public IComponentDeclaration GetDeclaration(ComponentIDEnumeration componentID) =>
			_ImplementedComponents.Find((component) => component.ID == componentID);

		/// <summary>
		/// Gets an <see cref="IComponentDeclaration"/> associated with the given type. If no declaration was found returns null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IComponentDeclaration GetDeclaration<T>() =>
			_AssociatedDeclarations.TryGetValue(typeof(T), out var declaration) ? declaration : null;

		/// <summary>
		/// Gets an <see cref="IComponentDeclaration"/> associated with the given type. If no declaration was found returns null
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IComponentDeclaration GetDeclaration(Type type) =>
			_AssociatedDeclarations.TryGetValue(type, out var declaration) ? declaration : null;

		/// <summary>
		/// Constructs and returns a component based on the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown if the interface does not correspond to a defined component or
		/// the interface is not a final implementation (eg. it's a <see cref="ITwoTerminal"/>)</exception>
		public IBaseComponent Construct<T>() where T : IBaseComponent
		{
			if (_AssociatedTypes.TryGetValue(typeof(T), out var result))
			{
				return Activator.CreateInstance(result) as IBaseComponent;
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
			if (_AssociatedInterfaces.TryGetValue(componentID, out var associatedInterface) &&
				_AssociatedTypes.TryGetValue(associatedInterface, out var result))
			{
				return Activator.CreateInstance(result) as IBaseComponent;				
			}
			
			throw new ArgumentException($"No component matches the ID: {componentID}");
		}

		/// <summary>
		/// Constructs a new <see cref="IWire"/>
		/// </summary>
		/// <returns></returns>
		public IWire ConstructWire() => new Wire();

		#endregion
	}
}