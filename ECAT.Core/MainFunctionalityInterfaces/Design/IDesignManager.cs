using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for design of a circuit
	/// </summary>
	public interface IDesignManager : INotifyPropertyChanged
    {
		#region Properties

		/// <summary>
		/// Collection of all active schematics
		/// </summary>
		ReadOnlyObservableCollection<ISchematic> Schematics { get; }

		/// <summary>
		/// The currently edited schematic
		/// </summary>
		ISchematic CurrentSchematic { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Creates and adds a new, empty schematic
		/// </summary>
		void AddSchematic();

		/// <summary>
		/// Adds the given schematic to <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		void AddSchematic(ISchematic schematic);

		/// <summary>
		/// Removes the given schematic from <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		void RemoveSchematic(ISchematic schematic);

		/// <summary>
		/// Removes schematic given by the argument
		/// </summary>
		/// <param name="index"></param>
		void RemoveSchematic(int index);

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one given
		/// </summary>
		/// <param name="schematic"></param>
		void ChangeCurrentSchematic(ISchematic schematic);

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one at <paramref name="index"/> in the <see cref="Schematics"/> collection
		/// </summary>
		/// <param name="index"></param>
		void ChangeCurrentSchematic(int index);

		/// <summary>
		/// Returns the type corresponding to the declaration
		/// </summary>
		/// <param name="declaration"></param>
		/// <returns></returns>
		Type GetComponentType(IComponentDeclaration declaration);

		/// <summary>
		/// Returns the type corresponding to the given component it
		/// </summary>
		/// <param name="declaration"></param>
		/// <returns></returns>
		Type GetComponentType(int id);		

		/// <summary>
		/// Method that removes a component from its <see cref="ISchematic"/>. First searches through the <see cref="CurrentSchematic"/>,
		/// if the <paramref name="component"/> is not found there searches through the rest of <see cref="Schematics"/>
		/// </summary>
		/// <param name="component"></param>
		void RemoveComponent(IBaseComponent component);

		/// <summary>
		/// Method that removes a wire from its <see cref="ISchematic"/>. First searches through the <see cref="CurrentSchematic"/>,
		/// if the <paramref name="wire"/> is not found there searches through the rest of <see cref="Schematics"/>
		/// </summary>
		/// <param name="wire"></param>
		void RemoveWire(IWire wire);

		#endregion
	}
}