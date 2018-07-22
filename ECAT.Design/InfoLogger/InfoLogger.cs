using ECAT.Core;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of the IInfoLogger interface
	/// </summary>
	public class InfoLogger : IInfoLogger, INotifyPropertyChanged
    {
		#region Events

		/// <summary>
		/// Event fired whenever one of the properties changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// Message that is currently presented
		/// </summary>
		public string Message { get; private set; }
		
		#endregion

		#region Public methods

		/// <summary>
		/// Logs a new message
		/// </summary>
		/// <param name="message"></param>
		public void Log(string message) => Message = message;

		#endregion
	}
}