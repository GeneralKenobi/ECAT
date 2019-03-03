using System.Collections.Generic;

namespace ECAT.Core
{
	[NecessaryService]
	public interface ICategorizedComponentCollectionProvider
	{
		#region Properties

		/// <summary>
		/// 
		/// </summary>
		IReadOnlyDictionary<ComponentCategory, IEnumerable<IComponentDeclaration>> Components { get; }


		#endregion
	}
}