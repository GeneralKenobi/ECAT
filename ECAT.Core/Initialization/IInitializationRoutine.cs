namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes that want to be called at start-up to perform initialization routine
	/// </summary>
	public interface IInitializationRoutine
    {
		#region Public methods

		/// <summary>
		/// Method called at app's startup
		/// </summary>
		void InitializationRoutine();

		#endregion
	}
}