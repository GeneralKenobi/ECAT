using System.ComponentModel;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Base class for view models or classes that want to implement <see cref="INotifyPropertyChanged"/>
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged
	{
		#region Property changed

		/// <summary>
		/// The event that is fired when any child property changed its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Protected methods

		/// <summary>
		/// Invokes Property Changed Event for each string parameter
		/// </summary>
		/// <param name="propertyName">Properties to invoke for. Null or string.Empty will result in notification for all properties</param>
		protected void InvokePropertyChanged(params string[] propertyNames)
		{
			foreach (var item in propertyNames)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item));
			}
		}

		#endregion
	}
}