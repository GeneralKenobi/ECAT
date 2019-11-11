namespace ECAT.Simulation
{
	/// <summary>
	/// Contains information about node indices for BJTs
	/// </summary>
	public class BjtNodeInfo
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="baseTerminal"></param>
		/// <param name="collectorTerminal"></param>
		/// <param name="emitterTerminal"></param>
		public BjtNodeInfo(int baseTerminal, int collectorTerminal, int emitterTerminal)
		{
			Base = baseTerminal;
			Collector = collectorTerminal;
			Emitter = emitterTerminal;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Base terminal
		/// </summary>
		public int Base { get; }

		/// <summary>
		/// Collector terminal
		/// </summary>
		public int Collector { get; }

		/// <summary>
		/// Emitter terminal
		/// </summary>
		public int Emitter { get; }

		#endregion
	}
}
