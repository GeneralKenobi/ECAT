namespace ECAT.Simulation
{
	/// <summary>
	/// Contains information about two terminal sources: node indices for two terminal sources
	/// </summary>
	public class TwoTerminalSourceNodeInfo
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		public TwoTerminalSourceNodeInfo(int positive, int negative)
		{
			Positive = positive;
			Negative = negative;
		}

		#endregion
	
		#region Public properties

		/// <summary>
		/// Positive terminal of the source (the 'front')
		/// </summary>
		public int Positive { get; }

		/// <summary>
		/// Negative terminal of the source (the 'back')
		/// </summary>
		public int Negative { get; }

		#endregion
	}
}