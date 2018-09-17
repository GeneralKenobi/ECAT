using System.Collections.Generic;
using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for factories of <see cref="IPhasorDomainSignal"/>
	/// </summary>
	public interface IPhasorDomainSignalFactory : ISignalDataFactory<IPhasorDomainSignal>
    {
		#region Methods

		/// <summary>
		/// Constructs an <see cref="IPhasorDomainSignal"/> with only a DC offset
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		IPhasorDomainSignal Construct(double dc);

		/// <summary>
		/// Constructs an <see cref="IPhasorDomainSignal"/> out of an enumeration of phasors and with DC offset equal to 0
		/// </summary>
		/// <param name="phasors"></param>
		/// <returns></returns>
		IPhasorDomainSignal Construct(IEnumerable<KeyValuePair<double, Complex>> phasors);

		/// <summary>
		/// Constructs an <see cref="IPhasorDomainSignal"/> with DC offset and an enumeration of phasors
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="phasors"></param>
		/// <returns></returns>
		IPhasorDomainSignal Construct(double dc, IEnumerable<KeyValuePair<double, Complex>> phasors);

		#endregion
	}
}