using System.Collections.Generic;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class containing info to present when a component is focused (for example when pointer is over it)
	/// </summary>
	public interface IComponentInfo : INotifyPropertyChanged
	{
		#region Properties

		/// <summary>
		/// Info to present from the currently section
		/// </summary>
		IEnumerable<string> CurrentSection { get; }

		/// <summary>
		/// Index of the currently selected section
		/// </summary>
		int CurrentSectionIndex { get; set; }

		/// <summary>
		/// Headers of all info sections
		/// </summary>
		IEnumerable<string> SectionHeaders { get; }

		#endregion
	}
}