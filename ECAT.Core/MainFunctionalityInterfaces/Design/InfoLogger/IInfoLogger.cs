namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that acts as a logger of information on screen, for example to present current action
	/// </summary>
	public interface IInfoLogger
	{
		#region Properties

		/// <summary>
		/// Message that is currently presented
		/// </summary>
		string Message { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Logs a new message
		/// </summary>
		/// <param name="message"></param>
		void Log(string message);

		#endregion
	}
}