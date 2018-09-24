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
		/// The presented info, interpreted and ready to be displayed on screen
		/// </summary>
		IEnumerable<string> InterpretedInfo { get; }

		/// <summary>
		/// The presented info
		/// </summary>
		ISignalInformation Info { get; }

		/// <summary>
		/// Headers of all info sections
		/// </summary>
		IEnumerable<IComponentInfoSectionHeader> SectionHeaders { get; }

		/// <summary>
		/// Number of all sections in this info
		/// </summary>
		int SectionsCount { get; }

		#endregion
	}
}