using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ECAT
{
	/// <summary>
	/// Convert which takes a normal rotation angle (anti clockwise) in degrees (as a double) and returns a clockwise rotation
	/// </summary>
	public class ReverseRotationAngleValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is double d)
			{
				return -d;
			}

			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
