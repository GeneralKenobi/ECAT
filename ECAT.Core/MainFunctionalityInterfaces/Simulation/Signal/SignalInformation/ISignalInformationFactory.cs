namespace ECAT.Core
{
	/// <summary>
	/// Interface for factories of <see cref="ISignalInformation"/>
	/// </summary>
	[NecessaryService]
	public interface ISignalInformationFactory
	{
		#region Methods

		/// <summary>
		/// Constructs an <see cref="ISignalInformation"/> based on <paramref name="data"/>
		/// </summary>
		/// <param name="data"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		ISignalInformation Construct(ISignalData data, ISignalDescription description);

		#endregion
	}
}