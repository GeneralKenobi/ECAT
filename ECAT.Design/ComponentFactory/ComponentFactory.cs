using ECAT.Core;
using System;

namespace ECAT.Design
{
	public class ComponentFactory : IComponentFactory
	{
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
