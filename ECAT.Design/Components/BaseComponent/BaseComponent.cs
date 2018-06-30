using CSharpEnhanced.Maths;
using ECAT.Core;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for all components
	/// </summary>
	public class BaseComponent : IBaseComponent
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// The center coordinate
		/// </summary>
		public cdouble Center { get; set; } = new cdouble();

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}