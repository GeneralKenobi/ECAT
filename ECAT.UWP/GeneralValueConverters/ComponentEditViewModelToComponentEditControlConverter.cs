using ECAT.ViewModel;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts a component view model to an appropriate component edit control
	/// </summary>
	public class ComponentEditViewModelToComponentEditControlConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts a component view model to an appropriate component edit control
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// Try to get the control type
			if(value != null && _AssociatedControls.TryGetValue(value.GetType(), out var controlType))
			{
				// If successful, create an instance
				var control = Activator.CreateInstance(controlType) as FrameworkElement;

				// Assign data context
				control.DataContext = value;

				// And return it
				return control;
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
		/// Dictionary containing types of controls associated with different ComponentEditViewModels
		/// </summary>
		private static Dictionary<Type, Type> _AssociatedControls { get; } = new Dictionary<Type, Type>()
		{
			{typeof(ResistorEditViewModel), typeof(ResistorEditUC) },
			{typeof(VoltageSourceEditViewModel), typeof(VoltageSourceEditUC) },
			{typeof(CurrentSourceEditViewModel), typeof(CurrentSourceEditUC) },
			{typeof(OpAmpEditViewModel), typeof(OpAmpEditUC) },
			{typeof(CapacitorEditViewModel), typeof(CapacitorEditUC) },
			{typeof(ACVoltageSourceEditViewModel), typeof(ACVoltageSourceEditUC) },
			{typeof(SweepVoltageSourceEditViewModel), typeof(SweepVoltageSourceEditUC) },
			{typeof(InductorEditViewModel), typeof(InductorEditUC) },
			{typeof(BjtEditViewModel), typeof(BjtEditUC) },
		};

		#endregion
	}
}