namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes containing raw data about a signal
	/// </summary>
	public interface ISignalData
	{
		#region Properties

		/// <summary>
		/// Object capable of interpreting the particular <see cref="ISignalData"/>
		/// </summary>
		ISignalDataInterpreter Interpreter { get; }

		/// <summary>
		/// Unit of this data
		/// </summary>
		string Unit { get; }

		#endregion
	}
}