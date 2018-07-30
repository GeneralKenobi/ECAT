using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Current source is an ideal source that can supply a chosen value of current to an arbitrary load
	/// </summary>
	public class CurrentSource : TwoTerminal, ICurrentSource
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public CurrentSource()
		{
			Admittance = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;
			ProducedCurrent = IoC.Resolve<IDefaultValues>().DefaultCurrentSourceProducedCurrent;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedCurrent { get; set; }

		#endregion
	}
}