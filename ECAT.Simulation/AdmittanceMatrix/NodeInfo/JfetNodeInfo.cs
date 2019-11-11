namespace ECAT.Simulation
{
	/// <summary>
	/// Contains information about node indices for JFETs
	/// </summary>
	public class JfetNodeInfo
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="gateTerminal"></param>
		/// <param name="drainTerminal"></param>
		/// <param name="sourceTerminal"></param>
		public JfetNodeInfo(int gateTerminal, int drainTerminal, int sourceTerminal)
		{
			Gate = gateTerminal;
			Drain = drainTerminal;
			Source = sourceTerminal;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gate terminal
		/// </summary>
		public int Gate { get; }

		/// <summary>
		/// Drain terminal
		/// </summary>
		public int Drain { get; }

		/// <summary>
		/// Source terminal
		/// </summary>
		public int Source { get; }

		#endregion
	}
}
