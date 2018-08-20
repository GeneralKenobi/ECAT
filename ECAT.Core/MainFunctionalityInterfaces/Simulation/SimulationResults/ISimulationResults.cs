﻿namespace ECAT.Core
{
	/// <summary>
	/// Interface for a class that manages and analyses results computed by <see cref="ISimulationManager"/>
	/// </summary>
	public interface ISimulationResults
    {
		#region Methods

		/// <summary>
		/// Gets a voltage drop of a node with respect to ground
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <returns></returns>
		bool TryGetVoltageDrop(int nodeIndex, out ISignalInformation voltageDrop);

		/// <summary>
		/// Gets information on voltage drop between two nodes (with node A being treated as the reference node). If the node
		/// indexes exceed currently held nodes count null is assigned to <paramref name="voltageDrop"/> and false is returned
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <returns></returns>
		bool TryGetVoltageDrop(int nodeAIndex, int nodeBIndex, out ISignalInformation voltageDrop);

		/// <summary>
		/// Gets a voltage drop of a node with respect to ground or returns a drop equal to zero if unsuccessful
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <returns></returns>
		ISignalInformation GetVoltageDropOrZero(int nodeIndex);

		/// <summary>
		/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns a drop
		/// equal to zero if unsuccessful
		/// </summary>
		/// <param name="nodeAIndex"></param>
		/// <param name="nodeBIndex"></param>
		/// <returns></returns>
		ISignalInformation GetVoltageDropOrZero(int nodeAIndex, int nodeBIndex);

		/// <summary>
		/// Gets information about current flowing through an <see cref="IResistor"/>
		/// </summary>
		/// <param name="voltageDrop"></param>
		/// <param name="resistor"></param>
		/// <returns></returns>
		bool TryGetCurrent(ISignalInformation voltageDrop, IResistor resistor, out ISignalInformation current);

		/// <summary>
		/// Gets information about current flowing through an <see cref="IResistor"/>
		/// </summary>
		/// <param name="voltageDrop"></param>
		/// <param name="resistor"></param>
		/// <returns></returns>
		bool TryGetCurrent(ISignalInformation voltageDrop, ICapacitor resistor, out ISignalInformation current);

		/// <summary>
		/// Gets information about power dissipated on an <see cref="IResistor"/>
		/// </summary>
		/// <param name="voltageDrop"></param>
		/// <param name="resistor"></param>
		/// <returns></returns>
		bool TryGetPower(ISignalInformation voltageDrop, IResistor resistor, out IPowerInformation power);

		/// <summary>
		/// Gets information about power on an <see cref="ICurrentSource"/>
		/// </summary>
		/// <param name="voltageDrop"></param>
		/// <param name="currentSource"></param>
		/// <param name="power"></param>
		/// <returns></returns>
		bool TryGetPower(ISignalInformation voltageDrop, ICurrentSource currentSource, out IPowerInformation power);
		
		#endregion
	}
}