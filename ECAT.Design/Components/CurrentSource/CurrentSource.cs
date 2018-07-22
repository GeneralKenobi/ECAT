using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Current source is an ideal source that can supply a chosen value of current to an arbitrary load
	/// </summary>
	public class CurrentSource : TwoTerminal, ICurrentSource
    {
		/// <summary>
		/// Default Constructor
		/// </summary>
		public CurrentSource()
		{
			Admittance = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;
		}
    }
}