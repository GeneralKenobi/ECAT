using ECAT.Core;

namespace ECAT.Design
{
	public class OpAmpDescription : ComponentDescription, IOpAmpDescription
	{
		#region Public properties

		/// <summary>
		/// Open loop gain of the described <see cref="IOpAmp"/>
		/// </summary>
		public double OpenLoopGain { get; set; }

		#endregion
	}
}