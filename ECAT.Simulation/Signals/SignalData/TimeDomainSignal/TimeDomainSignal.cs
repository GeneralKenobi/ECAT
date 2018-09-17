using CSharpEnhanced.CoreInterfaces;
using ECAT.Core;
using System.Collections.Generic;

namespace ECAT.Simulation
{
	/// <summary>
	/// Signal defined by a series of points with instantenous values for a specific time
	/// </summary>
	public partial class TimeDomainSignal : ITimeDomainSignal
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public TimeDomainSignal() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public TimeDomainSignal(ITimeDomainSignal signal)
		{
			Copy(signal);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Start time of the simulation, in seconds
		/// </summary>
		public double StartTime { get; set; }

		/// <summary>
		/// Time elapsed between two calculated values, in seconds
		/// </summary>
		public double TimeStep { get; set; }

		/// <summary>
		/// List with instantenous values. Key is the time, value is the signal's value.
		/// </summary>
		public IEnumerable<double> InstantenousValues { get; set; }

		/// <summary>
		/// Object capable of calculating characteristic values for this <see cref="ISignalData"/>
		/// </summary>
		public ISignalDataInterpreter Interpreter { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public TimeDomainSignal CopySignal() => new TimeDomainSignal(this);

		/// <summary>
		/// Copies internal state of <paramref name="obj"/> to this instance
		/// </summary>
		/// <param name="obj"></param>
		public void Copy(ITimeDomainSignal obj)
		{
			StartTime = obj.StartTime;
			TimeStep = obj.TimeStep;
			InstantenousValues = obj.InstantenousValues;
		}

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		ITimeDomainSignal IShallowCopyTo<ITimeDomainSignal>.Copy() => CopySignal();

		#endregion
	}
}