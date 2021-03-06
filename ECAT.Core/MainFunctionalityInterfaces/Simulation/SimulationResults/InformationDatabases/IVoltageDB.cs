﻿namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes holding information about voltage drops calculated in simulation
	/// </summary>
	public interface IVoltageDB
	{
		#region Methods

		/// <summary>
		/// Gets a voltage drop of a node with respect to ground or returns null if unsuccessful
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
		/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
		/// <returns></returns>
		ISignalInformation Get(int nodeIndex, bool nodeToGround = true);

		/// <summary>
		/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns null
		/// if unsuccessful
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <returns></returns>
		ISignalInformation Get(int nodeAIndex, int nodeBIndex);

		/// <summary>
		/// Gets information on voltage drop across a <see cref="ITwoTerminal"/> component
		/// </summary>
		/// <param name="component"></param>
		/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
		/// <returns></returns>
		ISignalInformation Get(ITwoTerminal component, bool voltageBA = true);

		#endregion
	}
}