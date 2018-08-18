using UWPEnhanced.Xaml;
using Windows.UI.Xaml;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts IsSelected (bool), Index (int) and SectionsCount (int) to diagonal border <see cref="Visibility"/>. If IsSelected
	/// is true and index equals sectionsCount - 1 then returns <see cref="Visibility.Collapsed"/>, otherwise always returns
	/// <see cref="Visibility.Visible"/>
	/// </summary>
	public class DiagonalVisibilityConverter : IMultiBindingValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts IsSelected (bool), Index (int) and SectionsCount (int) to diagonal border <see cref="Visibility"/>. If IsSelected
		/// is true and index equals sectionsCount - 1 then returns <see cref="Visibility.Collapsed"/>, otherwise always returns
		/// <see cref="Visibility.Visible"/>
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="value3"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public object Convert(object value1, object value2, object value3, object parameter)
		{
			// Check if arguments have correct types
			if (value1 is bool IsSelected && value2 is int index && value3 is int sectionsCount &&
				// If so if the header is selected and it is the last header, its diagonal shouldn't be visible
				IsSelected && index == sectionsCount - 1)
			{
				return Visibility.Collapsed;
			}

			// Otherwise return visible
			return Visibility.Visible;
		}

		#endregion
	}
}