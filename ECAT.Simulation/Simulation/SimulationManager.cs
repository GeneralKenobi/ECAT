﻿using ECAT.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of the simulation module
	/// </summary>
	public class SimulationManager : ISimulationManager
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public SimulationManager()
		{
			ImplementedComponents = new ReadOnlyObservableCollection<IComponentDeclaration>(_ImplementedComponents);
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="ImplementedComponents"/>
		/// TODO: When a file system is implemented, move the content to a file and read it from there
		/// </summary>
		private ObservableCollection<IComponentDeclaration> _ImplementedComponents { get; } =
			new ObservableCollection<IComponentDeclaration>()
		{
			new ComponentDeclaration(0, "Resistor", 2, ComponentType.Passive),
			new ComponentDeclaration(1, "Voltage Source", 2, ComponentType.Passive),
			new ComponentDeclaration(2, "Current Source", 2, ComponentType.Passive),
		};

		#endregion

		#region Public properties

		/// <summary>
		/// Collection of names of all components that are implemented and usable
		/// </summary>
		public ReadOnlyObservableCollection<IComponentDeclaration> ImplementedComponents { get; }

		/// <summary>
		/// A maximum value for parameters in the circuit (admittance, voltage source voltage, etc)
		/// </summary>
		public double MaximumParameterValue { get; } = 1e100;

		#endregion
	}
}