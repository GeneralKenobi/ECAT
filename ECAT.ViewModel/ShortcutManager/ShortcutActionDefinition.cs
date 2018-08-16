using CSharpEnhanced.CoreClasses;
using System;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Definition of a single shortcut action
	/// </summary>
	public class ShortcutActionDefinition
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ShortcutActionDefinition(string name, KeyArgument shortcut, Action action)
		{
			Name = name;
			DefinedKey = shortcut;
			Action = action;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The name of the action which is simultaneously the display name and a unique identifier
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The shortcut assigned to this definition
		/// </summary>
		public KeyArgument DefinedKey { get; set; }

		/// <summary>
		/// Action performed when <see cref="DefinedKey"/> criterions are met
		/// </summary>
		public Action Action { get; }

		#endregion
	}
}