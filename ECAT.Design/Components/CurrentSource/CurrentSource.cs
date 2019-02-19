using ECAT.Core;
using System.Numerics;

namespace ECAT.Design
{
	/// <summary>
	/// Current source is an ideal source that can supply a chosen value of current to an arbitrary load
	/// (<see cref="TwoTerminal.TerminalA"/> corresponds to the negative terminal and
	/// <see cref="TwoTerminal.TerminalB"/> corresponds to the positive terminal)
	/// </summary>
	[DisplayVoltageInfo(nameof(TerminalA), nameof(TerminalB), 0, "Voltage")]
	[DisplayPowerInfo(sectionIndex:1)]
	public class CurrentSource : TwoTerminal, ICurrentSource
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CurrentSource()
		{
			// Create a description
			_Description = new SourceDescription()
			{
				Label = this.Label,
				Frequency = 0,
				// Temporary index for now - TODO: change when CurrentSource has an active component index
				Index = -1,
				ComponentType = SourceType.DCCurrentSource,
			};
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Admittance of this <see cref="ICurrentSource"/> (constant value)
		/// </summary>
		private Complex _Admittance { get; } = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;

		/// <summary>
		/// Backing store for <see cref="Description"/>
		/// </summary>
		private SourceDescription _Description { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// Accessor to the current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedCurrent { get; set; } = IoC.Resolve<IDefaultValues>().DefaultCurrentSourceProducedCurrent;

		/// <summary>
		/// Description of this <see cref="ICurrentSource"/>
		/// </summary>
		public ISourceDescription Description => _Description;

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