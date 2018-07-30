using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converter which takes a normal rotation angle (anti clockwise) in degrees (as a double) and returns a clockwise rotation
	/// </summary>
	public class ReverseRotationAngleValueConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converter which takes a normal rotation angle (anti clockwise) in degrees (as a double) and returns a clockwise rotation
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is double d)
			{
				return -d;
			}

			return 0;
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