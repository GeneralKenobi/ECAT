namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes holding information about power dissipated/delivered by components
	/// </summary>
	public interface IPowerDB
	{
		#region Methods

		/// <summary>
		/// Gets information about power dissipated on an <see cref="IResistor"/>
		/// </summary>
		/// <param name="resistor"></param>
		/// <returns></returns>
		ISignalInformation Get(IResistor resistor, bool voltageBA);

		/// <summary>
		/// Gets information about power dissipated on an <see cref="ICapacitor"/>
		/// </summary>
		/// <param name="capacitor"></param>
		/// <returns></returns>
		ISignalInformation Get(ICapacitor capacitor, bool voltageBA);

		/// <summary>
		/// Gets information about power dissipated on an <see cref="IInductor"/>
		/// </summary>
		/// <param name="inductor"></param>
		/// <returns></returns>
		ISignalInformation Get(IInductor inductor, bool voltageBA);

		/// <summary>
		/// Gets information about power on an <see cref="ICurrentSource"/>
		/// </summary>
		/// <param name="currentSource"></param>
		/// <returns></returns>
		ISignalInformation Get(ICurrentSource currentSource, bool voltageBA);

		/// <summary>
		/// Gets information about power on an <see cref="IDCVoltageSource"/>
		/// </summary>
		/// <param name="voltageSource"></param>
		/// <returns></returns>
		ISignalInformation Get(IDCVoltageSource voltageSource, bool voltageBA);

		/// <summary>
		/// Gets information about power on an <see cref="IACVoltageSource"/>. If the <paramref name="current"/> is composed of
		/// phasors with different frequency than that of <paramref name="voltageSource"/> the average power will be assigned
		/// <see cref="Double.NaN"/> (it's impossible to calculate it using only phasors). Doesn't compute maximum/minimum
		/// instantenous power.
		/// </summary>
		/// <param name="voltageSource"></param>
		/// <returns></returns>
		ISignalInformation Get(IACVoltageSource voltageSource, bool voltageBA);

		#endregion
	}
}