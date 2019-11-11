namespace ECAT.Core
{
	/// <summary>
	/// Interface used for describing <see cref="IOpAmp"/>s
	/// </summary>
	public interface IOpAmpDescription : IComponentDescription
	{
		#region Properties

		/// <summary>
		/// Open loop gain of the described <see cref="IOpAmp"/>
		/// </summary>
		double OpenLoopGain { get; }

		#endregion
	}
}