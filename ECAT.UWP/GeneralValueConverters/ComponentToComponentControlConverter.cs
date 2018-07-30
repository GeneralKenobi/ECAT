using ECAT.Design;
using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts a component to an appropriate control
	/// </summary>
	public class ComponentToComponentControlConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts a component to an appropriate control
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
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

			if(value is CurrentSource)
			{
				return new CurrentSourceTC();
			}

			if (value is Ground)
			{
				return new GroundTC();
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