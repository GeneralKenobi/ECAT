using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for all components
	/// </summary>
	public class BaseComponent : INotifyPropertyChanged
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}