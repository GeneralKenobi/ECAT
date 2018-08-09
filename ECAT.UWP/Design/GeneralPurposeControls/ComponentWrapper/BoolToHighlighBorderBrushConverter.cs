using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts true to {StaticResource RedBrush} and everything else to transparent brush
	/// </summary>
	public class BoolToHighlighBorderBrushConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts true to {StaticResource RedBrush} and everything else to transparent brush
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool b && b)
			{
				return _BorderPresentBrush;
			}

			return new SolidColorBrush(Colors.Transparent);
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

		#region Private static properties

		/// <summary>
		/// Brush to return when true is passed as value to <see cref="Convert(object, Type, object, string)"/>
		/// </summary>
		private static Brush _BorderPresentBrush { get; } = App.Current.Resources["RedBrush"] as Brush;

		#endregion
	}
}