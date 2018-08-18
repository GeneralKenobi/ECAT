using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts true to {StaticResource WhiteBrush} and everything else to {StaticResource DarkBlueBrush30}
	/// </summary>
	public class IsSelectedToBackgroundBrushConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts true to {StaticResource WhiteBrush} and everything else to {StaticResource DarkBlueBrush30}
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is bool b && b)
			{
				return _WhiteBrush;
			}

			return _BlueBrush;
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
		/// White brush for true
		/// </summary>
		private static Brush _WhiteBrush { get; } = App.Current.Resources["WhiteBrush"] as Brush;

		/// <summary>
		/// Blue brush for everything else
		/// </summary>
		private static Brush _BlueBrush { get; } = App.Current.Resources["DarkBlueBrush30"] as Brush;

		#endregion
	}
}