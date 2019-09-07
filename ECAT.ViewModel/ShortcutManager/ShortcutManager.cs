using CSharpEnhanced.CoreClasses;
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
		#region Constructors

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
		public List<Tuple<string, KeyArgument>> RegisteredShortcuts => new List<Tuple<string, KeyArgument>>(
			_RegisteredShortcuts.Select((x) => new Tuple<string, KeyArgument>(x.Name, x.DefinedKey)));

		#endregion

		#region Private methods

		/// <summary>
		/// Updates the shortcuts without checking if they're correct
		/// </summary>
		/// <param name="newBindings"></param>
		private void UpdateKeyShortcutsList(List<Tuple<string, KeyArgument>> newBindings)
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
				"Place Resistor", new KeyArgument("R"), () => PlacePartHelper(ComponentIDEnumeration.Resistor)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Voltage Source", new KeyArgument("V"), () => PlacePartHelper(ComponentIDEnumeration.VoltageSource)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Current Source", new KeyArgument("C"), () => PlacePartHelper(ComponentIDEnumeration.CurrentSource)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Stop Current Action", new KeyArgument("Escape"), () => AppViewModel.Singleton.DesignVM.StopActionCommand?.Execute(null)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Loose Wire", new KeyArgument("W"), () => AppViewModel.Singleton.DesignVM.PrepareToPlaceLooseWireCommand.Execute(null)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Ground", new KeyArgument("G"), () => PlacePartHelper(ComponentIDEnumeration.Ground)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place OpAmp", new KeyArgument("O"), () => PlacePartHelper(ComponentIDEnumeration.OpAmp)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Sweep Voltage Source", new KeyArgument("S"), () => PlacePartHelper(ComponentIDEnumeration.SweepVoltageSource)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place AC Voltage Source", new KeyArgument("V", KeyModifiers.Shift), () => PlacePartHelper(ComponentIDEnumeration.ACVoltageSource)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Voltmeter", new KeyArgument("V", KeyModifiers.Ctrl), () => PlacePartHelper(ComponentIDEnumeration.Voltmeter)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Capacitor", new KeyArgument("C", KeyModifiers.Shift), () => PlacePartHelper(ComponentIDEnumeration.Capacitor)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place Inductor", new KeyArgument("L"), () => PlacePartHelper(ComponentIDEnumeration.Inductor)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Place NPN BJT", new KeyArgument("N"), () => PlacePartHelper(ComponentIDEnumeration.NpnBjt)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run DC Bias", new KeyArgument("T"), () => AppViewModel.Singleton.SimulationVM.SimulationManager.DCBias(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));
			
			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run Full Cycle AC Without Op-Amp Adjustment",
				new KeyArgument("Z"), () => AppViewModel.Singleton.SimulationVM.SimulationManager.ACFullCycleWithoutOperationAdjustment(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run Full Cycle ACDC Without Op-Amp Adjustment",
				new KeyArgument("X"), () => AppViewModel.Singleton.SimulationVM.SimulationManager.ACDCFullCycleWithoutOperationAdjustment(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run Full Cycle AC With Op-Amp Adjustment",
				new KeyArgument("Z", KeyModifiers.Shift), () => AppViewModel.Singleton.SimulationVM.SimulationManager.ACFullCycleWithOperationAdjustment(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run Full Cycle ACDC With Op-Amp Adjustment",
				new KeyArgument("X", KeyModifiers.Shift), () => AppViewModel.Singleton.SimulationVM.SimulationManager.ACDCFullCycleWithOperationAdjustment(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));

			_RegisteredShortcuts.Add(new ShortcutActionDefinition(
				"Run Full Cycle ACDC With Op-Amp Adjustment",
				new KeyArgument("Z", KeyModifiers.Ctrl), () => AppViewModel.Singleton.SimulationVM.SimulationManager.FrequencySweep(AppViewModel.Singleton.DesignVM.DesignManager.CurrentSchematic)));

			// TODO: Change the ShortcutKeys to ShortcutKey.Empty and load the saved combinations from a local file
		}

		/// <summary>
		/// Method used as a helper when performing quick actions based on placing parts
		/// </summary>
		/// <param name="componentID"></param>
		private void PlacePartHelper(ComponentIDEnumeration componentID) => AppViewModel.Singleton.DesignVM.ComponentToAdd =
			IoC.Resolve<IComponentFactory>().GetDeclaration(componentID);
		
		#endregion

		#region Public methods

		/// <summary>
		/// Assigns new shortcuts (given as Item2 in the Tuple) to all <see cref="ShortcutActionDefinition"/>
		/// with the same Name as Item1 of the Tuple. Checks if the new key mapping are correct (each key combination
		/// occurs at most one time, if not reverts to the previous combination and throws an exception, if yes
		/// saves the new key mapping to a file.
		/// </summary>
		/// <param name="newBindings"></param>
		public void UpdateKeyShortcuts(List<Tuple<string, KeyArgument>> newBindings)
		{
			// Get a backup collection
			var backup = RegisteredShortcuts;

			// Update the shortcut keys with the new bindings
			UpdateKeyShortcutsList(newBindings);

			// Get hashes of all ShortcutKeys except the empty ones
			var hashes = new List<int>(_RegisteredShortcuts.Where((x) => !x.Equals(KeyArgument.Empty)).Select((x) =>
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
					"or more actions (excluding " + nameof(KeyArgument.Empty) + ")");
			}
		}		

		/// <summary>
		/// Processes a key combination and, if shortcuts are registered for it, executes all found occurances
		/// </summary>
		/// <param name="key"></param>
		/// <param name="activeModifiers"></param>
		public void ProcessKeyCombination(KeyArgument pressedKey) =>
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