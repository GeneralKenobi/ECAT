using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.ViewModel
{
	/// <summary>
	/// ViewModel for display of specific information about <see cref="ITimeDomainSignal"/>s
	/// </summary>
	public class TimeDomainSignalViewModel : BaseViewModel
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="signal"></param>
		/// <param name="unit">The unit of values</param>
		/// <exception cref="ArgumentNullException"></exception>
		public TimeDomainSignalViewModel(ITimeDomainSignal signal, string unit)
		{
			if(signal == null)
			{
				throw new ArgumentNullException(nameof(signal));
			}

			Data = signal.InstantenousValues.
				Select((value, counter) => new KeyValuePair<double, double>(signal.StartTime + counter * signal.TimeStep, value));

			YUnit = unit ?? throw new ArgumentNullException(nameof(unit));
		}

		#endregion

		#region Public properties
		
		/// <summary>
		/// Text ready to be displayed on screen, contains detailed information about the <see cref="IPhasorDomainSignal"/>
		/// </summary>
		public IEnumerable<KeyValuePair<double, double>> Data { get; private set; }

		/// <summary>
		/// Unit for time (horizontal axis)
		/// </summary>
		public string XUnit { get; } = IoC.Resolve<ISIUnits>().Time;

		/// <summary>
		/// Unit for values
		/// </summary>
		public string YUnit { get; }

		#endregion
	}
}