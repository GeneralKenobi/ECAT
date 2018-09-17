namespace ECAT.Core.GenericFactories
{
	/// <summary>
	/// Interface for factories allowing for parameterless construction of an instance of <see cref="T"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFactoryParameterless<T>
	{
		#region Methods

		/// <summary>
		/// Constructs a <see cref="T"/>
		/// </summary>
		/// <returns></returns>
		T Construct();

		#endregion
	}
}