namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing information about a signal - voltage drop or current flow
	/// </summary>
	public interface ISignalInformation : ISignal
    {
		#region Properties

		/// <summary>
		/// True if the direction of signal was inverted (with respect to assumed directions) to present <see cref="Maximum"/> as a
		/// positive number
		/// </summary>
		bool InvertedDirection { get; }

		/// <summary>
		/// The maximum instantenous signal value that may occur
		/// </summary>
		double Maximum { get; }

		/// <summary>
		/// The minimum instantenous signal value that may occur
		/// </summary>
		double Minimum { get; }

		/// <summary>
		/// RMS value of this signal
		/// </summary>
		double RMS { get; }

		/// <summary>
		/// The type of the signal
		/// </summary>
		SignalType Type { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Copies all contents of <paramref name="signalInformation"/> to this object.
		/// </summary>
		/// <param name="signalInformation"></param>
		void CopyFrom(ISignalInformation signalInformation);

		/// <summary>
		/// Returns a copy of this instance
		/// </summary>
		/// <returns></returns>
		ISignalInformation Copy();

		#endregion
	}
}