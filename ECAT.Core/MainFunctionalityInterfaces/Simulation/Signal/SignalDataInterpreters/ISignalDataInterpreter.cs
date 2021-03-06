﻿namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes exposing methods returning characteristic values of some <see cref="ISignalData"/>.
	/// </summary>
	public interface ISignalDataInterpreter
    {
		#region Methods

		/// <summary>
		/// Calculates and returns the maximum instantenous value of the signal
		/// </summary>
		/// <returns></returns>
		double Maximum();

		/// <summary>
		/// Calculates and returns the minimum instantenous value of the signal
		/// </summary>
		/// <returns></returns>
		double Minimum();

		/// <summary>
		/// Calculates and returns the root-mean-square value of the signal
		/// </summary>
		/// <returns></returns>
		double RMS();

		/// <summary>
		/// Calculates and returns the average value
		/// </summary>
		/// <returns></returns>
		double Average();

		#endregion
	}
}