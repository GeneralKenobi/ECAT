using System;

namespace ECAT.Core
{
	/// <summary>
	/// ID lable uniquely identifying <see cref="IBaseComponent"/>s
	/// </summary>
	[NecessaryService]
	public interface IIDLabel : IDisposable
	{
		#region Properties

		/// <summary>
		/// Unique label assigned to the component
		/// </summary>
		string Label { get; }

		#endregion

		#region Methods

		/// <summary>
		/// If <paramref name="label"/> is unique then assigns it as new <see cref="Label"/> and returns true, otherwise returns false.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		bool Update(string label);

		#endregion
	}
}