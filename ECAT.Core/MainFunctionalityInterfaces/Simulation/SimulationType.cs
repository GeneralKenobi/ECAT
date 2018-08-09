namespace ECAT.Core
{
	/// <summary>
	/// Enumerates types of simulations
	/// </summary>	
	public enum SimulationType
    {
		/// <summary>
		/// Simulation was pure DC
		/// </summary>
		DC = 1,

		/// <summary>
		/// Simulations was pure AC
		/// </summary>
		AC = 2,

		/// <summary>
		/// Simulation was DC as well as AC
		/// </summary>
		ACDC = 3,
    }
}