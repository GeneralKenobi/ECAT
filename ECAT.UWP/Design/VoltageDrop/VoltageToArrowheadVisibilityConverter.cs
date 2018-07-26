using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	public class VoltageToArrowheadVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is double d)
			{
				return parameter == null ? (d >= 0 ? Visibility.Visible : Visibility.Collapsed) :
					(d >= 0 ? Visibility.Collapsed : Visibility.Visible);
			}

			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
