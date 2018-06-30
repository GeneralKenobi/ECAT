using PropertyChanged;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Base class for view models
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged
	{
		#region Property Changed

		/// <summary>
		/// The event that is fired when any child property changed its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;		

		#endregion
	}
}