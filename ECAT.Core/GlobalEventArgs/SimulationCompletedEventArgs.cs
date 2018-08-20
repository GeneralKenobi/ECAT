using System;

namespace ECAT.Core
{
	/// <summary>
	/// Arguments for an event fired whenever simulation completes
	/// </summary>
	public class SimulationCompletedEventArgs : EventArgs
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="simulationType"></param>
		public SimulationCompletedEventArgs(SimulationType simulationType)
		{
			SimulationType = SimulationType;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The type of the carried out simulation
		/// </summary>
		SimulationType SimulationType { get; }

		#endregion
	}
}