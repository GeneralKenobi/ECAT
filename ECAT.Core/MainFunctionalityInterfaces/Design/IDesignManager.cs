using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for design of a circuit
	/// </summary>
    public interface IDesignManager : INotifyPropertyChanged
    {
		#region Properties

		/// <summary>
		/// Collection of components added to the circuit
		/// </summary>
		ReadOnlyObservableCollection<IBaseComponent> Components { get; }

		/// <summary>
		/// Collection of wires added to the circuit
		/// </summary>
		ReadOnlyObservableCollection<IWire> Wires { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Method that adds a new component to the circuit
		/// </summary>
		/// <param name="component"></param>
		void AddNewComponent(IBaseComponent component);

		/// <summary>
		/// Method that adds a new wire to the circuit
		/// </summary>
		/// <param name="wire"></param>
		void AddNewWire(IWire wire);

		/// <summary>
		/// Method that removes a component from the circuit
		/// </summary>
		/// <param name="component"></param>
		void RemoveComponent(IBaseComponent component);

		/// <summary>
		/// Method that removes a wire from the circuit
		/// </summary>
		/// <param name="wire"></param>
		void RemoveWire(IWire wire);

		#endregion
	}
}
