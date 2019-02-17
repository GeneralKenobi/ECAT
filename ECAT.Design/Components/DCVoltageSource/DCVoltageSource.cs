using ECAT.Core;
using System.Numerics;
using System.Collections.Generic;

namespace ECAT.Design
{
	/// <summary>
	/// Component representing an ideal voltage source (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	[DisplayCurrentInfo(sectionIndex: 1)]
	[DisplayPowerInfo(sectionIndex: 2)]
	public class DCVoltageSource : TwoTerminal, IDCVoltageSource
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public DCVoltageSource()
		{
			// Create a description
			_Description = new ActiveComponentDescription()
			{
				Label = this.Label,
				Index = ActiveComponentIndex,
				Frequency = 0,
				ComponentType = ActiveComponentType.DCVoltageSource,
			};
		}

		#endregion

		#region Private properties

		/// <summary>
		/// The admittance of a voltage source (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().VoltageSourceAdmittance;

		/// <summary>
		/// Backing store for <see cref="Description"/>
		/// </summary>
		private ActiveComponentDescription _Description { get; }

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="ActiveComponentIndex"/>
		/// </summary>
		private int mActiveComponentIndex;

		#endregion

		#region Public properties		

		/// <summary>
		/// DC voltage produced by this <see cref="IDCVoltageSource"/>
		/// </summary>
		public double ProducedDCVoltage { get; set; } = IoC.Resolve<IDefaultValues>().DefaultVoltageSourceProducedVoltage;

		/// <summary>
		/// Index used to query <see cref="ISimulationResults"/> for produced current
		/// </summary>
		public int ActiveComponentIndex
		{
			get => mActiveComponentIndex;
			set
			{
				// Update the backing store and value in description
				mActiveComponentIndex = value;
				_Description.Index = value;
			}
		}

		/// <summary>
		/// Description of this <see cref="IActiveComponent"/>
		/// </summary>
		public IActiveComponentDescription Description => _Description;

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns the admittance of an <see cref="IDCVoltageSource"/>
		/// </summary>
		/// <param name="frequency"></param>
		/// <returns></returns>
		protected override Complex CalculateAdmittance(double frequency) => _Admittance;

		#endregion
	}
}