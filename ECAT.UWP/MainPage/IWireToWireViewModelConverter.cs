using ECAT.Core;
using ECAT.ViewModel;
using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts an <see cref="IWire"/>
	/// </summary>
	public class IWireToWireViewModelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is IWire wire)
			{
				return new WireViewModel(wire);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
