using CSharpEnhanced.CoreInterfaces;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes presenting information about some <see cref="ISignalData"/>
	/// </summary>
	public interface ISignalInformation : IDeepCopy<ISignalInformation>
	{
		#region Properties

		/// <summary>
		/// Raw data of the signal
		/// </summary>
		ISignalData Data { get; }

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
		/// The average value of the signal
		/// </summary>
		double Average { get; }

		#endregion
	}
}