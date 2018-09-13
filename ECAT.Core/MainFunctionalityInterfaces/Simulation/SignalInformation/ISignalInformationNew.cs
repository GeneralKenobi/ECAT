using CSharpEnhanced.CoreInterfaces;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes presenting information about some <see cref="ISignalData"/>
	/// </summary>
	public interface ISignalInformationNew : IDeepCopy<ISignalInformationNew>
	{
		#region Properties

		/// <summary>
		/// Raw data of the signal
		/// </summary>
		ISignalData Data { get; }

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

		#endregion
	}
}