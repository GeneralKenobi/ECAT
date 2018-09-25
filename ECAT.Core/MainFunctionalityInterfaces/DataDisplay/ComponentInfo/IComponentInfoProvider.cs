﻿namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class providing component info to display
	/// </summary>
	public interface IComponentInfoProvider
	{
		#region Properties

		/// <summary>
		/// The info to present
		/// </summary>
		IComponentInfo Value { get; }

		/// <summary>
		/// Moves the info to next section
		/// </summary>
		void GoToNextSection();

		#endregion
	}
}