using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Current source is an ideal source that can supply a chosen value of current to an arbitrary load
	/// (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public class CurrentSource : TwoTerminal, ICurrentSource
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentSource() : base(new string[] { "Voltage", "Delivered power"}) { }

		#endregion

		#region Private properties

		/// <summary>
		/// Admittance of this <see cref="ICurrentSource"/> (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;

		#endregion

		#region Public properties

		/// <summary>
		/// Accessor to the current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedCurrent { get; set; } = IoC.Resolve<IDefaultValues>().DefaultCurrentSourceProducedCurrent;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns info related to power
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<string> GetPowerInfo()
		{
			var dcPower = _VoltageDrop.DC * ProducedCurrent;
			
			var maxPower = _VoltageDrop.Maximum * ProducedCurrent;		

			// Return characteristic power information
			yield return "Maximum instantenous power: " +
				SIHelpers.ToSIStringExcludingSmallPrefixes(maxPower.RoundToDigit(4), "W");

			// Average power is just the DC power - AC power averages to 0
			yield return "Average power: " + SIHelpers.ToSIStringExcludingSmallPrefixes(dcPower.RoundToDigit(4), "W");
		}

		/// <summary>
		/// Returns info related to this current source which is voltage info plus power info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo() =>
			new IEnumerable<string>[] { GetVoltageInfo(), GetPowerInfo() };

		#endregion

		#region Public methods

		/// <summary>
		/// Returns the admittance of this <see cref="ICurrentSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		#endregion
	}
}