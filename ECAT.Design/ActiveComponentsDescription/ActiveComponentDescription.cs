using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IActiveComponentDescription"/> - this class describes <see cref="IActiveComponent"/>s.
	/// </summary>
	public class ActiveComponentDescription : IActiveComponentDescription
	{
		#region Public Properties

		/// <summary>
		/// Unique label assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// Active component index assigned to the <see cref="IActiveComponent"/> that is described by this instance
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Frequency of the described <see cref="IActiveComponent"/>
		/// </summary>
		public double Frequency { get; set; }

		/// <summary>
		/// Peak value (voltage or current) produced by the described <see cref="IActiveComponent"/>
		/// </summary>
		public double PeakProducedValue { get; set; }

		/// <summary>
		/// Type of the <see cref="IActiveComponent"/>
		/// </summary>
		public ActiveComponentType ComponentType { get; set; }

		#endregion
	}
}