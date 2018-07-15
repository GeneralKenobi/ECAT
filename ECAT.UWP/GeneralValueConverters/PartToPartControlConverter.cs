using ECAT.Core;
using ECAT.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	public class PartToPartControlConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is Resistor)
			{
				return new ResistorTC();
			}

			if(value is VoltageSource)
			{
				return new VoltageSourceTC();
			}

			//if (value is BasePart part && ControlsHelpers.TryGetUIControl(part, out FrameworkElement element))
			//{
			//	return element;
			//}
			//else
			//{
			//	return null;
			//}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
