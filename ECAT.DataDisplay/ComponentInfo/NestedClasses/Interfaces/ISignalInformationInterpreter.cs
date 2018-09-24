using ECAT.Core;
using System.Collections.Generic;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Interprets an <see cref="ISignalInformation"/> (converts from <see cref="ISignalInformation"/> to a sequence of strings
		/// that can be displayed on screen)
		/// </summary>
		private interface ISignalInformationInterpreter
		{
			#region Methods

			/// <summary>
			/// Converts the <see cref="ISignalInformation"/> to a sequence of strings that can be displayed on screen.
			/// For null <paramref name="info"/> returns an empty sequence.
			/// </summary>
			/// <param name="info"></param>
			/// <returns></returns>
			IEnumerable<string> Get(ISignalInformation info);

			#endregion
		}
	}
}