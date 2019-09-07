using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Base interface for transistors
	/// </summary>
	public interface ITransistor : IThreeTerminal
	{
		#region Properties

		/// <summary>
		/// If true, small-signal model of the transistor will be used
		/// </summary>
		bool SmallSignalModel { get; set; }

		#endregion
	}
}