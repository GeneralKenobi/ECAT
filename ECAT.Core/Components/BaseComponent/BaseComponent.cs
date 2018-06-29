using CSharpEnhanced.Maths;
using System.ComponentModel;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for all components
	/// </summary>
	public class BaseComponent : INotifyPropertyChanged
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

		#endregion
	}
}