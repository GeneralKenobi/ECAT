namespace ECAT.Core
{
	/// <summary>
	/// Base interface for factories of <see cref="ISignalData"/>s
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISignalDataFactory<T>
		where T : ISignalData
    {
		#region Methods

		/// <summary>
		/// Constructs a <see cref="T"/> equal to 0
		/// </summary>
		/// <returns></returns>
		T Construct();

		/// <summary>
		/// Constructs a shallow copy of <paramref name="signal"/>
		/// </summary>
		/// <param name="signal"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		T Construct(T signal);

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