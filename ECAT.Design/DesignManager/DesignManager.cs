using ECAT.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Default implementation of <see cref="IDesignManager"/>
	/// </summary>
	public class DesignManager : IDesignManager
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public DesignManager()
		{
			Schematics = new ReadOnlyObservableCollection<ISchematic>(_Schematics);
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Backing store for <see cref="Schematic"/>
		/// </summary>
		private ObservableCollection<ISchematic> _Schematics { get; } = new ObservableCollection<ISchematic>();

		#endregion

		#region Public Properties

		/// <summary>
		/// Collection of all active schematics
		/// </summary>
		public ReadOnlyObservableCollection<ISchematic> Schematics { get; }

		/// <summary>
		/// The currently edited schematic
		/// </summary>
		public ISchematic CurrentSchematic { get; private set; }

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public methods

		/// <summary>
		/// Creates and adds a new, empty schematic
		/// </summary>
		public void AddSchematic() => _Schematics.Add(new Schematic());

		/// <summary>
		/// Adds the given schematic to <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		public void AddSchematic(ISchematic schematic) => _Schematics.Add(schematic);

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one given
		/// </summary>
		/// <param name="schematic"></param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="schematic"/> is not a part of <see cref="Schematics"/>
		/// </exception>
		public void ChangeCurrentSchematic(ISchematic schematic)
		{
			if(_Schematics.Contains(schematic))
			{
				CurrentSchematic = schematic;
			}
			else
			{
				throw new ArgumentException(nameof(schematic) + " is not a part of this " + nameof(IDesignManager));
			}
		}

		/// <summary>
		/// Changes the <see cref="CurrentSchematic"/> to the one at <paramref name="index"/> in the <see cref="Schematics"/>
		/// collection
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void ChangeCurrentSchematic(int index)
		{
			if(index < 0 || index >= _Schematics.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			CurrentSchematic = _Schematics[index];
		}

		/// <summary>
		/// Removes the given schematic from <see cref="Schematics"/>
		/// </summary>
		/// <param name="schematic"></param>
		public void RemoveSchematic(ISchematic schematic)
		{
			_Schematics.Remove(schematic);
			
			// If the removed schematic was the current one, set the current one to null
			if(CurrentSchematic == schematic)
			{
				CurrentSchematic = null;
			}
		}

		/// <summary>
		/// Removes schematic given by the argument
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void RemoveSchematic(int index)
		{
			if (index < 0 || index >= _Schematics.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			RemoveSchematic(_Schematics[index]);
		}

		#endregion
	}
}