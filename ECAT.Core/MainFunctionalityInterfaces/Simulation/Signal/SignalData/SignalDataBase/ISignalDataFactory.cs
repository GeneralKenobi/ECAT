namespace ECAT.Core
{
	/// <summary>
	/// Base interface for factories of <see cref="ISignalData"/>s
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[NecessaryService]
	public interface ISignalDataFactory<T> : IFactoryParameterless<T>, IFactoryShallowCopying<T>
		where T : ISignalData
    {
		#region Methods

		/// <summary>
		/// Returns a negation (shallow copy) of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		T ConstructNegation(T signal);

		#endregion
	}
}