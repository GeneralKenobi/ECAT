using ECAT.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Simulation
{
	/// <summary>
	/// Implementation of the simulation module
	/// </summary>
	public class Simulation : ISimulation
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Simulation()
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
		};

		#endregion

		#region Public properties

		/// <summary>
		/// Collection of names of all components that are implemented and usable
		/// </summary>
		public ReadOnlyObservableCollection<IComponentDeclaration> ImplementedComponents { get; }

		#endregion
	}
}