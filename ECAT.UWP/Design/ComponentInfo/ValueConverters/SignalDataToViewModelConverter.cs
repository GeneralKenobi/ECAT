using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using ECAT.ViewModel;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace ECAT.UWP
{
	/// <summary>
	/// Converter for <see cref="ComponentInfoUC"/>, converts specific <see cref="ISignalData"/> to viewmodels
	/// </summary>
	public class SignalDataToViewModelConverter : IValueConverter
    {
		#region Public methods

		/// <summary>
		/// Converts specific <see cref="ISignalData"/> to viewmodels
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			object result = null;
			var values = new double[20].Select((x, i) => Math.Pow(10, 4) * Math.Sin(i));
			return new TimeDomainSignalViewModel(IoC.Resolve<ITimeDomainSignal>(values, 1e-5d,0d), "V");

			TypeSwitch.Construct().
				LazyCase<IPhasorDomainSignal>((x) => result = new PhasorDomainSignalViewModel(x, IoC.Resolve<ISIUnits>().VoltageShort)).
				LazyCase<ITimeDomainSignal>((x) => result = new TimeDomainSignalViewModel(x, IoC.Resolve<ISIUnits>().VoltageShort)).
				Switch(value);

			return result;
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