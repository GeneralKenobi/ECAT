using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// A Factory producing <see cref="IBaseComponent"/> 
	/// </summary>
    public interface IComponentFactory
    {
		#region Methods

		/// <summary>
		/// Constructs and returns a component based on the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown if the interface does not correspond to a defined component or
		/// the interface is not a final implementation (eg. it's a <see cref="ITwoTerminal"/>)</exception>
		IBaseComponent Construct<T>() where T : IBaseComponent;

		/// <summary>
		/// Constructs a new <see cref="IBaseComponent"/> based on the given <see cref="IComponentDeclaration"/>
		/// </summary>
		/// <param name="declaration"></param>
		/// <returns></returns>
		IBaseComponent Construct(IComponentDeclaration declaration);

		/// <summary>
		/// Constructs a new <see cref="IBaseComponent"/> based on the given component id
		/// </summary>
		/// <param name="componentID"></param>
		/// <returns></returns>
		IBaseComponent Construct(int componentID);

		/// <summary>
		/// Constructs a new <see cref="IWire"/>
		/// </summary>
		/// <returns></returns>
		IWire ConstructWire();

		#endregion
	}
}