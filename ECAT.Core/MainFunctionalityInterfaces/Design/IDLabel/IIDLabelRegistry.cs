namespace ECAT.Core
{
	/// <summary>
	/// Manages <see cref="IIDLabel"/> labels - keeps track of used labels and provides default labels.
	/// </summary>
	[NecessaryService]
	public interface IIDLabelRegistry
	{
		#region Methods

		/// <summary>
		/// Tries to register <paramref name="label"/> with the registry. If it's free it will be added to the registry (reserved) and method will
		/// return true (meaning it can be used), otherwise it will return false.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		bool TryRegister(string label);

		/// <summary>
		/// Returns next free default label
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		string GetNextDefault();

		/// <summary>
		/// Frees the given label (removes reservation). Nothing happens if the label was not reserved.
		/// </summary>
		/// <param name="label"></param>
		void Free(string label);

		#endregion
	}
}