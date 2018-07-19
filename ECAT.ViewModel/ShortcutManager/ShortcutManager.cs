using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ECAT.ViewModel
{
	public class ShortcutManager
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ShortcutManager()
		{
			PopulateRegisteredShortcuts();
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Collection containing all registered shortcuts and their appropriate actions
		/// </summary>
		private List<ShortcutActionDefinition> _RegisteredShortcuts { get; } = new List<ShortcutActionDefinition>();

		#endregion

		#region Public properties

		/// <summary>
		/// Collection of all registered shortcuts. The <see cref="string"/> element is simulataneously the display name and a unique
		/// ID of the <see cref="ShortcutActionDefinition"/> and <see cref="ShortcutKey"/> is the assigned shortcut
		/// </summary>
		public List<Tuple<string, ShortcutKey>> RegisteredShortcuts => new List<Tuple<string, ShortcutKey>>(
			_RegisteredShortcuts.Select((x) => new Tuple<string, ShortcutKey>(x.Name, x.DefinedKey)));

		#endregion

		#region Private methods

		/// <summary>
		/// Populates the <see cref="_RegisteredShortcuts"/> collection with <see cref="ShortcutActionDefinition"/>s,
		/// should be called as soon as possible, preferably from the constructor
		/// </summary>
		private void PopulateRegisteredShortcuts()
		{
			// Clear the old shortcuts definitions
			_RegisteredShortcuts.Clear();

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Resistor", new ShortcutKey("R"), () => PlacePartHelper(ComponentIDEnumeration.Resistor)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Voltage Source", new ShortcutKey("V"), () => PlacePartHelper(ComponentIDEnumeration.Resistor)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Current Source", new ShortcutKey("C"), () => PlacePartHelper(ComponentIDEnumeration.Resistor)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Stop Current Action", new ShortcutKey("Escape"), StopActions));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Loose Wire", new ShortcutKey("W"), PlaceLooseWire));

			// TODO: Change the ShortcutKeys to ShortcutKey.Empty and load the saved combinations from a local file
		}

		/// <summary>
		/// Method used as a helper when performing quick actions based on placing parts
		/// </summary>
		/// <param name="componentID"></param>
		private void PlacePartHelper(ComponentIDEnumeration componentID) => AppViewModel.Singleton.DesignVM.ComponentToAdd =
			IoC.Resolve<IComponentFactory>().ImplementedComponents.First((x) => x.ID == componentID);

		/// <summary>
		/// Helper method, stops currently performed action
		/// </summary>
		private void StopActions() => AppViewModel.Singleton.DesignVM.StopActionCommand?.Execute(null);

		/// <summary>
		/// Helper method, prepares the program to place a loose wire
		/// </summary>
		private void PlaceLooseWire() => AppViewModel.Singleton.DesignVM.PrepareToPlaceLooseWireCommand.Execute(null);

		#endregion

		#region Public methods

		/// <summary>
		/// Assigns new shortcuts (given as Item2 in the Tuple) to all <see cref="ShortcutActionDefinition"/>
		/// with the same Name as Item1 of the Tuple
		/// </summary>
		/// <param name="newBindings"></param>
		public void UpdateShortcuts(List<Tuple<string, ShortcutKey>> newBindings)
		{
			// For each new binding
			newBindings.ForEach((newBinding) =>
			{
				// Check if it is defined
				var oldBinding = _RegisteredShortcuts.Find((y) => y.Name == newBinding.Item1);

				if (oldBinding != null)
				{
					// If so, update the shortcut key
					oldBinding.DefinedKey = newBinding.Item2;
				}
			});
		}		

		/// <summary>
		/// Processes a key combination and, if shortcuts are registered for it, executes all found occurances
		/// </summary>
		/// <param name="key"></param>
		/// <param name="activeModifiers"></param>
		public void ProcessKeyCombination(ShortcutKey pressedKey) =>
			_RegisteredShortcuts.FindAll((x) => x.DefinedKey == pressedKey).ForEach((x) => x.Action.Invoke());

		#endregion
	}
}