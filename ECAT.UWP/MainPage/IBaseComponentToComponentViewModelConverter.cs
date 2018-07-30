using ECAT.Core;
using ECAT.ViewModel;
using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts an <see cref="IBaseComponent"/> to a <see cref="ComponentViewModel"/>
	/// </summary>
	public class IBaseComponentToComponentViewModelConverter : IValueConverter
    {
		#region Public methods

		/// <summary>
		/// Converts an <see cref="IBaseComponent"/> to a <see cref="ComponentViewModel"/>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is IBaseComponent component)
			{
				return new ComponentViewModel(component);
			}

			return null;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}