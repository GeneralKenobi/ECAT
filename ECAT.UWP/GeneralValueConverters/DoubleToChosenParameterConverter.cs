using System;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// If value is a double and it's positive: checks if parameter is a string, if it contains a backslash ("/"),
	/// splits the parameter on the backslash, if there were at least two substrings created tries to parse both of them to ints.
	/// If all this is successfull returns the int on the left of the backslash if value was nonnegative, otherwise returns the
	/// int on the right of the backslash.
	/// If any of the conditions described above is not met returns null.
	/// </summary>
	public class DoubleToChosenParameterConverter : IValueConverter
    {
		#region Public methods

		/// <summary>
		/// If value is a double and it's positive: checks if parameter is a string, if it contains a backslash ("/"),
		/// splits the parameter on the backslash, if there were at least two substrings created tries to parse both of them to ints.
		/// If all this is successfull returns the int on the left of the backslash if value was nonnegative, otherwise returns the
		/// int on the right of the backslash.
		/// If any of the conditions described above is not met returns null.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// Check if value is a double, if parameter is a string and if it contains a backslash
			if (value is double d && parameter is string sParam && sParam.Contains("/"))
			{
				// Split the parameter
				var split = sParam.Split("/");

				// If it's long enough and the first two substrings can be parsed to int
				if(split.Length >= 1 && int.TryParse(split[0], out int column1) && int.TryParse(split[1], out int column2))
				{
					// Return the first one if the value is nonnegative, otherwise return the other one
					return d >= 0 ? column1 : column2;
				}
			}
			
			// If something went wrong return null
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