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
		/// <param name="description"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public ISignalInformation Construct(ISignalData data, ISignalDescription description) =>
			new SignalInformation(data, description);

		#endregion
	}
}