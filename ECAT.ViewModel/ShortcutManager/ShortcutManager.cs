using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Class acting as a manager for shortcuts - handles key presses, stores user settings locally, checks to make sure
	/// all defined shortcuts are valid
	/// </summary>
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
		/// Updates the shortcuts without checking if they're correct
		/// </summary>
		/// <param name="newBindings"></param>
		private void UpdateKeyShortcutsList(List<Tuple<string, ShortcutKey>> newBindings)
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
				"Place Voltage Source", new ShortcutKey("V"), () => PlacePartHelper(ComponentIDEnumeration.VoltageSource)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Current Source", new ShortcutKey("C"), () => PlacePartHelper(ComponentIDEnumeration.CurrentSource)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Stop Current Action", new ShortcutKey("Escape"), () => AppViewModel.Singleton.DesignVM.StopActionCommand?.Execute(null)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Loose Wire", new ShortcutKey("W"), () => AppViewModel.Singleton.DesignVM.PrepareToPlaceLooseWireCommand.Execute(null)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run Single DC Sweep", new ShortcutKey("T"), () => AppViewModel.Singleton.SimulationVM.SimulationManager.SingleDCSweep(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));

			// TODO: Change the ShortcutKeys to ShortcutKey.Empty and load the saved combinations from a local file
		}

		/// <summary>
		/// Method used as a helper when performing quick actions based on placing parts
		/// </summary>
		/// <param name="componentID"></param>
		private void PlacePartHelper(ComponentIDEnumeration componentID) => AppViewModel.Singleton.DesignVM.ComponentToAdd =
			IoC.Resolve<IComponentFactory>().ImplementedComponents.First((x) => x.ID == componentID);
		
		#endregion

		#region Public methods

		/// <summary>
		/// Assigns new shortcuts (given as Item2 in the Tuple) to all <see cref="ShortcutActionDefinition"/>
		/// with the same Name as Item1 of the Tuple. Checks if the new key mapping are correct (each key combination
		/// occurs at most one time, if not reverts to the previous combination and throws an exception, if yes
		/// saves the new key mapping to a file.
		/// </summary>
		/// <param name="newBindings"></param>
		public void UpdateKeyShortcuts(List<Tuple<string, ShortcutKey>> newBindings)
		{
			// Get a backup collection
			var backup = RegisteredShortcuts;

			// Update the shortcut keys with the new bindings
			UpdateKeyShortcutsList(newBindings);

			// Get hashes of all ShortcutKeys except the empty ones
			var hashes = new List<int>(_RegisteredShortcuts.Where((x) => !x.Equals(ShortcutKey.Empty)).Select((x) =>
				x.DefinedKey.GetHashCode()));

			// Get a distinct collection of the hashes and check if it's count is equal to the count of all hashes
			if(hashes.Distinct().Count() == hashes.Count)
			{
				// If so, then it means that all key bingins are unique and the new values may be applied and saved

				// TODO: Save the new bindings locally
			}
			else
			{
				// If not, then the new key bindings are not correct: they must be reverted and an exception must be thrown
				UpdateKeyShortcutsList(backup);

				throw new ArgumentException("After modifying the shortcut collection with new key bindings the " +
					"bindings are no longer unique (there is at least one key combination that is associated with two " +
					"or more actions (excluding " + nameof(ShortcutKey.Empty) + ")");
			}
		}		

		/// <summary>
		/// Processes a key combination and, if shortcuts are registered for it, executes all found occurances
		/// </summary>
		/// <param name="key"></param>
		/// <param name="activeModifiers"></param>
		public void ProcessKeyCombination(ShortcutKey pressedKey) =>
			_RegisteredShortcuts.Find((x) => x.DefinedKey.Equals(pressedKey))?.Action.Invoke();

		/// <summary>
		/// Loads the default key mappings from a file and saves them as current ones
		/// </summary>
		public void RevertToDefault()
		{
			// TODO: Load the default key mappings from a file and save them as the current ones
		}

		#endregion
	}
}