using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class providing component info to display
	/// </summary>
	[NecessaryService]
	public interface IComponentInfoProvider : INotifyPropertyChanged
	{
		#region Properties

		/// <summary>
		/// The info to present
		/// </summary>
		IComponentInfo Value { get; }

		/// <summary>
		/// True if the current info doesn't provide any value and can be not displayed
		/// </summary>
		bool CanBeHidden { get; }

		/// <summary>
		/// Moves the info to next section
		/// </summary>
		void GoToNextSection();

		#endregion
	}
}