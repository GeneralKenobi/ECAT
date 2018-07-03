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
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is IBaseComponent component)
			{
				return new ComponentViewModel(component);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}