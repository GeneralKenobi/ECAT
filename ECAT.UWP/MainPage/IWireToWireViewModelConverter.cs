using ECAT.Core;
using ECAT.ViewModel;
using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts an <see cref="IWire"/> to its view model
	/// </summary>
	public class IWireToWireViewModelConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts an <see cref="IWire"/> to its view model
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is IWire wire)
			{
				return new WireViewModel(wire);
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