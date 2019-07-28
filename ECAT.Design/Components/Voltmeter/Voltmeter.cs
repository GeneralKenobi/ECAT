using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of an IVoltmeter
	/// </summary>
	public class Voltmeter : TwoTerminal, IVoltmeter
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Voltmeter()
		{
			this.ID = _IDCounter;
			++_IDCounter;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The ID number of this Voltmeter (used to differentiate between generated voltage plots)
		/// </summary>
		public int ID { get; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns admittance of a voltmeter (always 0 - infinite resistance)
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => 0;

		#endregion

		#region Private statis properties

		/// <summary>
		/// Counter representing the next ID to use
		/// </summary>
		private static int _IDCounter { get; set; } = 1;

		#endregion
	}
}
