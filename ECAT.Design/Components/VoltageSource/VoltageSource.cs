using ECAT.Core;
using System.Numerics;
using System.Collections.Generic;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	public class VoltageSource : TwoTerminal, IVoltageSource
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltageSource()
		{
			// Reverse directions by default - voltage source usually produces current that flows in the same direction as is the
			// direction of the produced voltage drop
			ChangeVIDirections = true;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		#endregion
		
		#region Public properties		

		/// <summary>
		/// DC voltage produced by this <see cref="IVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

		/// <summary>
		/// Index used to query <see cref="ISimulationResults"/> for produced current
		/// </summary>
		public int ActiveComponentIndex { get; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of an <see cref="IVoltageSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;
		
		#endregion
	}
}