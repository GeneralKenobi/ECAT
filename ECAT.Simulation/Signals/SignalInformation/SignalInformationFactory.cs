using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Factory of <see cref="ISignalInformation"/>s.
	/// </summary>
	[RegisterAsInstance(typeof(ISignalInformationFactory))]
	public class SignalInformationFactory : ISignalInformationFactory
    {
		#region Public methods

		/// <summary>
		/// Constructs an <see cref="ISignalInformation"/> based on <paramref name="data"/>
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public ISignalInformation Construct(ISignalData data) => new SignalInformation(data);

		#endregion
	}
}