using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for factories of <see cref="ITimeDomainSignal"/>
	/// </summary>
	public interface ITimeDomainSignalFactory : ISignalDataFactory<ITimeDomainSignal>
	{
		#region Methods

		/// <summary>
		/// Constructs a new <see cref="ITimeDomainSignal"/>
		/// </summary>
		/// <param name="instantenousValues">Values occuring at specific time moments, can't be null</param>
		/// <param name="timeStep">Time step between two subsequent values</param>
		/// <param name="startTime">Start time of the signal</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		ITimeDomainSignal Construct(IEnumerable<double> instantenousValues, double timeStep, double startTime = 0);

		#endregion
	}
}