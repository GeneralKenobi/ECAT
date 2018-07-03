using ECAT.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ECAT.Design
{
	public class Schematic : ISchematic
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Schematic()
		{
			Components = new ReadOnlyObservableCollection<IBaseComponent>(_Components);
			Wires = new ReadOnlyObservableCollection<IWire>(_Wires);
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Backing store for <see cref="Components"/>
		/// </summary>
		private ObservableCollection<IBaseComponent> _Components { get; } = new ObservableCollection<IBaseComponent>();

		/// <summary>
		/// Backing store for <see cref="Wires"/>
		/// </summary>
		private ObservableCollection<IWire> _Wires { get; } = new ObservableCollection<IWire>();

		#endregion

		#region Public Properties

		/// <summary>
		/// Collection of components added to the circuit
		/// </summary>
		public ReadOnlyObservableCollection<IBaseComponent> Components { get; }

		/// <summary>
		/// Collection of wires added to the circuit
		/// </summary>
		public ReadOnlyObservableCollection<IWire> Wires { get; }

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public Methods

		/// <summary>
		/// Method that adds a new component to the circuit
		/// </summary>
		/// <param name="component"></param>
		public void AddComponent(IBaseComponent component) => _Components.Add(component);

		/// <summary>
		/// Method that adds a new wire to the circuit
		/// </summary>
		/// <param name="wire"></param>
		public void AddWire(IWire wire) => _Wires.Add(wire);

		/// <summary>
		/// Method that removes a component from the circuit
		/// </summary>
		/// <param name="component"></param>
		public void RemoveComponent(IBaseComponent component)
		{
			component.Dispose();
			_Components.Remove(component);
		}

		/// <summary>
		/// Method that removes a wire from the circuit
		/// </summary>
		/// <param name="wire"></param>
		public void RemoveWire(IWire wire)
		{
			wire.Dispose();
			_Wires.Remove(wire);
		}

		#endregion
	}
}