using ECAT.Core;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Control interface for functionalities of <see cref="IFocusManager"/>
	/// </summary>
	internal interface IFocusManagerControl
	{
		#region Methods

		/// <summary>
		/// Sets focus to <paramref name="component"/>
		/// </summary>
		/// <param name="component"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void SetFocus(IBaseComponent component);

		/// <summary>
		/// Loses focus of <paramref name="component"/> (<paramref name="component"/> has to match the currently focused component,
		/// otherwise focus won't be lost).
		/// </summary>
		/// <param name="component"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void LoseFocus(IBaseComponent component);

		#endregion
	}
}