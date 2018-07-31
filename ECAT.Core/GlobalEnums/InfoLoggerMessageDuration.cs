namespace ECAT.Core
{
	/// <summary>
	/// Enumeration for duration of messages logged in an <see cref="IInfoLogger"/>
	/// </summary>
	public enum InfoLoggerMessageDuration
    {
		/// <summary>
		/// The message will be presented until it is either removed or a new message is logged
		/// </summary>
		Infinite = 0,

		/// <summary>
		/// The message will be shown briefly (approximately 5 seconds), just to quickly signal something to the user
		/// </summary>
		Short = 1,

		/// <summary>
		/// The message will be shown for a moderate amount of time (approximately 10 seconds)
		/// </summary>
		Medium = 2,

		/// <summary>
		/// The message will be shown for a long amount of time (approximately 15 seconds)
		/// </summary>
		Long = 3,
    }
}