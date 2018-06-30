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

		#endregion
	}
}