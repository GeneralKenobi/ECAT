using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// If value is double and it is equal to 180 (upside down rotation) returns 180. Used to fix rotation of text in rotatable controls
	/// </summary>
	public class TextPositionFixRotationConverter : IValueConverter
    {
		#region Public methods

		/// <summary>
		/// If value is double and it is equal to 180 (upside down rotation) returns 180
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is double d && d == 180)
			{
				return 180;
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