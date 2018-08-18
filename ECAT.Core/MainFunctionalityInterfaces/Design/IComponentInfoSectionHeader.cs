using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Inteface for headers in component's info section
	/// </summary>
	public interface IComponentInfoSectionHeader : INotifyPropertyChanged
    {
		#region Properties

		/// <summary>
		/// Zero-based index
		/// </summary>
		int Index { get; }

		/// <summary>
		/// Text to display
		/// </summary>
		string Text { get; }

		/// <summary>
		/// True if this header's section is selected
		/// </summary>
		bool IsSelected { get; }

		#endregion
	}
}