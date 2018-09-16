using ECAT.Core;
using System.Collections.Generic;
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
		public CurrentSource() : base(new string[] { IoC.Resolve<IQuantityNames>().VoltageCap, IoC.Resolve<IQuantityNames>().PowerCap}) { }

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
		protected IEnumerable<string> GetPowerInfo(IPowerInformation info)
		{
			// Return characteristic power information
			yield return CIFormat.LineInfo("Minimum instantenous " + IoC.Resolve<IQuantityNames>().Power,
				info.Minimum, IoC.Resolve<ISIUnits>().PowerShort);
			yield return CIFormat.LineInfo("Maximum instantenous " + IoC.Resolve<IQuantityNames>().Power,
				info.Maximum, IoC.Resolve<ISIUnits>().PowerShort);
			yield return CIFormat.LineInfo("Average " + IoC.Resolve<IQuantityNames>().Power,
				info.Average, IoC.Resolve<ISIUnits>().PowerShort);
		}

		/// <summary>
		/// Returns info related to this current source which is voltage info plus power info
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IEnumerable<string>> GetComponentInfo()
		{
			yield return GetVoltageInfo(_VoltageDrop);
			yield return GetPowerInfo(IoC.Resolve<ISimulationResults>().GetPower(this));
		}

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