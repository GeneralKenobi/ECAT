using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Represents a key combination that may be used as a shortcut key - a key and some modifiers
	/// </summary>
	public class ShortcutKey
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		public ShortcutKey(string key, KeyModifiers modifiers = KeyModifiers.None)
		{
			Key = key;
			ActiveModifiers = modifiers;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Key which invokes the shortcut, it is not case sensitive. Full names are always used (eg. Control instead of Ctrl)
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// Modifiers that need to be active when the key is pressed
		/// </summary>
		public KeyModifiers ActiveModifiers { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Compares two <see cref="ShortcutKey"/>s, if both public properties are equal then the objects are equal.
		/// Keys are not case sensitive.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(obj is ShortcutKey shortcut)
			{
				return Key.ToUpper() == shortcut.Key.ToUpper() && ActiveModifiers == shortcut.ActiveModifiers;
			}

			return false;
		}

		/// <summary>
		/// Returns a unique hash code for the instance
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => Key.ToUpper().GetHashCode() * 17 + ActiveModifiers.GetHashCode();

		#endregion

		#region Public static properties

		/// <summary>
		/// Undefined shortcut
		/// </summary>
		public static ShortcutKey Empty { get; } = new ShortcutKey(string.Empty);

		#endregion
	}
}