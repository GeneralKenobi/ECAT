namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that acts as a logger of information on screen, for example to present current action
	/// </summary>
	[NecessaryService]
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
		/// Logs a new message anonymously (the message won't be removable by the means of <see cref="RemoveLog(int)"/>.
		/// The message has to contain at least one non-white space character to be logged. To remove messages
		/// use <see cref="RemoveLog(int)"/>
		/// </summary>
		/// <param name="message"></param>		
		/// <param name="duration">Maximum lifetime of the message, if the message was still presented after the time expires,
		/// it will be removed. By default it's infinite</param>
		void Log(string message, InfoLoggerMessageDuration duration = InfoLoggerMessageDuration.Infinite);

		/// <summary>
		/// Logs a new message. The message has to contain at least one non-white space character to be logged. To remove messages
		/// use <see cref="RemoveLog(int)"/>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="loggerID">ID used to identify the caller. Messages may be removed only if the same ID is used later</param>
		/// <param name="duration">Maximum lifetime of the message, if the message was still presented after the time expires,
		/// it will be removed. By default it's infinite</param>
		void Log(string message, int loggerID, InfoLoggerMessageDuration duration = InfoLoggerMessageDuration.Infinite);

		/// <summary>
		/// Removes the currently presented message if the given ID matches the one given while logging the message
		/// </summary>
		/// <param name="loggerID"></param>
		void RemoveLog(int loggerID);

		#endregion
	}
}