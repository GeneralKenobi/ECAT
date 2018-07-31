using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

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

		#region Private properties

		/// <summary>
		/// The ID of the last logger (the currently displayed message)
		/// </summary>
		private int _LastLoggerID { get; set; }

		/// <summary>
		/// ID of the last message that was logged with a limited time duration
		/// </summary>
		private int _LastTimedMessageID { get; set; }

		/// <summary>
		/// Random number generator for <see cref="_LastTimedMessageID"/>
		/// </summary>
		private Random _RandomNumberGenerator { get; } = new Random();

		/// <summary>
		/// Dictionary with durations assigned to specific <see cref="InfoLoggerMessageDuration"/>
		/// </summary>
		private Dictionary<InfoLoggerMessageDuration, TimeSpan> _DefinedDurations { get; } =
			new Dictionary<InfoLoggerMessageDuration, TimeSpan>()
			{
				{InfoLoggerMessageDuration.Short, TimeSpan.FromSeconds(5) },
				{InfoLoggerMessageDuration.Medium, TimeSpan.FromSeconds(10) },
				{InfoLoggerMessageDuration.Long, TimeSpan.FromSeconds(15) },

				// Include an entry for infinite so as not to crash by accident, although if the assumption that no delay is run
				// for infinite duration is kept this value will never be used
				{InfoLoggerMessageDuration.Infinite, TimeSpan.MaxValue },
			};

		#endregion

		#region Public properties

		/// <summary>
		/// Message that is currently presented
		/// </summary>
		public string Message { get; private set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Runs the delay for the given caller. Obtains a new <see cref="_LastTimedMessageID"/>. If, after the duration, the value
		/// didn't change, removes the message (using <see cref="RemoveLog(int)"/>)s
		/// </summary>
		/// <param name="loggerID"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		private async Task MessageDelayAsync(int loggerID, InfoLoggerMessageDuration duration)
		{
			// If the duration is infinite simply return
			if (duration == InfoLoggerMessageDuration.Infinite)
			{
				return;
			}

			// Get a unique ID
			var messageID = _RandomNumberGenerator.Next();

			// Assign it to the controling property
			_LastTimedMessageID = messageID;

			// Wait for the duration of the delay
			await Task.Delay(_DefinedDurations[duration]);

			// If the ID remained the same
			if(messageID == _LastTimedMessageID)
			{
				// Remove the log
				RemoveLog(loggerID);

				// And reset the last id
				_LastTimedMessageID = 0;
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Logs a new message
		/// </summary>
		/// <param name="message"></param>
		public void Log(string message, int loggerID)
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				Message = message;
				_LastLoggerID = loggerID;
			}
		}

		/// <summary>
		/// Logs a new message. The message has to contain at least one non-white space character to be logged. To remove messages
		/// use <see cref="RemoveLog(int)"/>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="loggerID">ID used to identify the caller. Messages may be removed only if the same ID is used later</param>
		/// <param name="duration">Maximum lifetime of the message, if the message was still presented after the time expires,
		/// it will be removed. By default it's infinite</param>
		public void Log(string message, int loggerID, InfoLoggerMessageDuration duration = InfoLoggerMessageDuration.Infinite)
		{
			// Log the message
			Log(message, loggerID);
			
			// Run the delay task
			MessageDelayAsync(loggerID, duration);
		}

		/// <summary>
		/// Removes the currently presented message if the given ID matches the one given while logging the message
		/// </summary>
		/// <param name="loggerID"></param>
		public void RemoveLog(int loggerID)
		{
			if(loggerID == _LastLoggerID)
			{
				Message = string.Empty;
				_LastLoggerID = 0;
			}
		}

		#endregion
	}
}