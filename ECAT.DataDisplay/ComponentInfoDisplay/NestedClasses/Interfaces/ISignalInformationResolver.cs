using ECAT.Core;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Interface for classes that resolve some signal information for base components
		/// </summary>
		private interface ISignalInformationResolver
		{
			#region Public methods

			/// <summary>
			/// Gets some signal information associated with <paramref name="target"/>
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			/// <exception cref="System.ArgumentNullException"></exception>
			ISignalInformation Get(IBaseComponent target);

			#endregion
		}
	}
}