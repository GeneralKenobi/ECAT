﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// If value is a double returns Visible for values >= 0, collapsed otherwise. If parameter is not null
	/// returns collapes for values >=0, visible otherwise (opposite to null parameter).
	/// If value is not a double always returns collapsed.
	/// </summary>
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