using ECAT.Core;
using System;
using System.Collections.Generic;
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
		/// Converts a component to an appropriate component edit control
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// Try to get the control type
			if (value != null && _AssociatedControls.TryGetValue(IoC.Resolve<IComponentFactory>().GetDeclaration(value.GetType()).ID,
				out var controlType))
			{
				// If successful, create an instance and return it
				return Activator.CreateInstance(controlType);
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

		#region Private static properties

		/// <summary>
		/// Dictionary containing types of controls associated with different <see cref="ComponentIDEnumeration"/>s
		/// </summary>
		private static Dictionary<ComponentIDEnumeration, Type> _AssociatedControls { get; } =
			new Dictionary<ComponentIDEnumeration, Type>()
		{
			{ComponentIDEnumeration.Resistor, typeof(ResistorTC) },
			{ComponentIDEnumeration.VoltageSource, typeof(VoltageSourceTC) },
			{ComponentIDEnumeration.CurrentSource, typeof(CurrentSourceTC) },
			{ComponentIDEnumeration.Ground, typeof(GroundTC) },
		};

		#endregion
	}
}