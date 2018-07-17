using ECAT.Core;
using System;

namespace ECAT.Design
{
	public class ComponentFactory : IComponentFactory
	{
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
			if (typeof(T) == typeof(IResistor)) return new Resistor();
			if (typeof(T) == typeof(IVoltageSource)) return new VoltageSource();
			if (typeof(T) == typeof(ICurrentSource)) return new CurrentSource();

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
		public IBaseComponent Construct(int componentID)
		{
			switch (componentID)
			{
				case 0: return new Resistor();

				case 1: return new VoltageSource();

				case 2: return new CurrentSource();

				default:
					{
						throw new ArgumentException($"No component matches the ID: {componentID}");
					}
			}
		}

		/// <summary>
		/// Constructs a new <see cref="IWire"/>
		/// </summary>
		/// <returns></returns>
		public IWire ConstructWire() => new Wire();
	}
}