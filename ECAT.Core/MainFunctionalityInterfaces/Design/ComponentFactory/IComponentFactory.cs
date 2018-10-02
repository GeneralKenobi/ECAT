using System;
using System.Collections.ObjectModel;

namespace ECAT.Core
{
	/// <summary>
	/// A Factory producing <see cref="IBaseComponent"/> 
	/// </summary>
	[NecessaryService]
	public interface IComponentFactory
    {
		#region Properties

		/// <summary>
		/// Collection of names of all components that are implemented and usable
		/// </summary>
		ReadOnlyCollection<IComponentDeclaration> ImplementedComponents { get; }

		#endregion

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
		IBaseComponent Construct(ComponentIDEnumeration componentID);

		/// <summary>
		/// Constructs a new <see cref="IWire"/>
		/// </summary>
		/// <returns></returns>
		IWire ConstructWire();

		/// <summary>
		/// Returns declaration of a component based on its ID
		/// </summary>
		/// <param name="componentID"></param>
		/// <returns></returns>
		IComponentDeclaration GetDeclaration(ComponentIDEnumeration componentID);

		/// <summary>
		/// Gets an <see cref="IComponentDeclaration"/> associated with the given type. If no declaration was found returns null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IComponentDeclaration GetDeclaration<T>();

		/// <summary>
		/// Gets an <see cref="IComponentDeclaration"/> associated with the given type. If no declaration was found returns null
		/// </summary>		
		/// <param name="type"></param>
		/// <returns></returns>
		IComponentDeclaration GetDeclaration(Type type);

		#endregion
	}
}