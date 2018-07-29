using ECAT.Design;
using ECAT.ViewModel;
using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	public class ComponentEditViewModelToComponentEditControlConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is ResistorEditViewModel)
			{
				return new ResistorEditUC()
				{
					DataContext = value,
				};
			}

			//if(value is VoltageSource)
			//{
			//	return new VoltageSourceTC();
			//}

			//if(value is CurrentSource)
			//{
			//	return new CurrentSourceTC();
			//}

			//if (value is Ground)
			//{
			//	return new GroundTC();
			//}

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
